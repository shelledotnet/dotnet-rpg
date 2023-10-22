using dotnet_rpg.AttributeUsed;
using dotnet_rpg.domain.Dtos;
using dotnet_rpg.domain.Models;
using dotnet_rpg.Extensions;
using Microsoft.AspNetCore.DataProtection;
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
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ServiceFailedResponse))]
    [TypeFilter(typeof(ApiKeyAttribute))]
    public class AuthController : ControllerBase
    {

        private static readonly User user = new();
        private readonly ProjectOptions _projectOptions;
        public AuthController(IOptionsMonitor<ProjectOptions> projectOptions)
        {
            _projectOptions=projectOptions.CurrentValue;
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ServiceResponse<User>))]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDto   userDto)
        {
            ServiceResponse<User> response = new ServiceResponse<User>
            {
                Data = user,
                Message = "success",
                Success = true
            };

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetApiResponse());
            }





            CreatePasswordHash(userDto.Password, out byte[] passwordHash, out byte[] passwordSalt);

            user.Username= userDto.Username;
            user.Role = userDto.Role;
            user.PasswordSalt = passwordSalt;
            user.PasswordHash = passwordHash;

            return Ok(response);
        }

        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ServiceFailedResponse))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ServiceResponse<TokenResponse>))]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto userDto)
        {
           
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetApiResponse());
            }
            if(user.Username != userDto.Username)
            {
                return BadRequest(new ServiceFailedResponse { Message = "user wasn't found", Success=false });
            }
            if(!VerifyPasswordHash(userDto.Password,user.PasswordHash,user.PasswordSalt))
                return BadRequest(new ServiceFailedResponse { Message = "invalid user login details", Success = false });


            ServiceResponse<GenToken> response = new ServiceResponse<GenToken>
            {
                Data = CreateToken(user),
                Message = "success",
                Success = true
            };



            return Ok(response);
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
        private GenToken CreateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
           var claims = new ClaimsIdentity(new[] 
            {
                new Claim(ClaimTypes.Name,user.Username),
            });

            foreach (var item in user.Role)
            {
                claims.AddClaim(new Claim(ClaimTypes.Role, item));
            }

            var key = Convert.FromBase64String(_projectOptions.SecreteKey);
            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);
     
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                #region CustomeClaims
                Subject = claims,
                #endregion
                #region GenericClaims
                Issuer = _projectOptions.Issuer,
                Audience = _projectOptions.Audience,
                //Expires = DateTime.Now.AddMinutes(5),
                Expires = DateTime.Now.Add(_projectOptions.TokenLifeTime),
                SigningCredentials = signingCredentials,
                #endregion

            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return new GenToken
            {
                Token = tokenHandler.WriteToken(token),
             Username = user.Username,
                ValidTo = token.ValidTo.AddHours(1),
                ValidFrom = token.ValidFrom

            };
            
        }

    }
}
