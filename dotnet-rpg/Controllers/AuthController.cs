using dotnet_rpg.domain.Dtos;
using dotnet_rpg.domain.Models;
using dotnet_rpg.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace dotnet_rpg.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private static readonly User user = new();
        private readonly ProjectOptions _projectOptions;
        public AuthController(IOptionsMonitor<ProjectOptions> projectOptions)
        {
            _projectOptions=projectOptions.CurrentValue;
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(User))]
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register([FromBody] UserDto   userDto)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetApiResponse());
            }





            CreatePasswordHash(userDto.Password, out byte[] passwordHash, out byte[] passwordSalt);

            user.Username= userDto.Username;
            user.PasswordSalt = passwordSalt;
            user.PasswordHash = passwordHash;

            return Ok(user);
        }

        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(FailedRequest))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TokenResponse))]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserDto userDto)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetApiResponse());
            }
            if(user.Username != userDto.Username)
            {
                return BadRequest(new FailedRequest { Message = "user wasn't found", Status = false });
            }
            if(!VerifyPasswordHash(userDto.Password,user.PasswordHash,user.PasswordSalt))
                return BadRequest(new FailedRequest { Message = "invalid user login details", Status = false });

            ;
            return Ok(new TokenResponse {Token= CreateToken(user),Username=user.Username });
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512(passwordSalt);
            byte[] computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(passwordHash);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        private string CreateToken(User user)
        {
            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.Name,user.Username)

            };
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_projectOptions.SecreteKey));
            var  credential   =  new SigningCredentials(key,SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                signingCredentials: credential,
                expires: DateTime.Now.AddMinutes(5)
            );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

    }
}
