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
    [Produces("application/json", "application/xml")]  //output formatter Media type: Accept header
    [Consumes("application/json")] //input-formatter Media type: content-type header
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ServiceFailedResponse))]
    [TypeFilter(typeof(ApiKeyAttribute))]
    public class AuthController : ControllerBase
    {
        //mimiking dependency injection IOC
        private static readonly User user = new();
        private static readonly RefereshTokenModel refreshToken = new();
        private readonly ProjectOptions _projectOptions;
        public AuthController(IOptionsMonitor<ProjectOptions> projectOptions)
        {
            _projectOptions=projectOptions.CurrentValue;
        }

        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ServiceResponse<User>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ServiceBadResponse))]
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
            user.Role = userDto.Role.Select(x => x.ToLower()).ToList();
            user.PasswordSalt = passwordSalt;
            user.PasswordHash = passwordHash;

            return Created("",response);
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


        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ServiceResponse<TokenResponse>))]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            var refereshToken = Request.Cookies["refreshToken"];
            if (!user.RefreshToken.Equals(refereshToken))
            {
                ServiceFailedResponse serviceFailedResponse = new ServiceFailedResponse
                {
                    Message = "invalid refresh token",
                    Success = false
                };
                return StatusCode(401, serviceFailedResponse);
            }
            else if (user.TokenExpired < DateTime.Now)
            {
                ServiceFailedResponse serviceFailedResponse = new ServiceFailedResponse
                {
                    Message = "token expired",
                    Success = false
                };
                return StatusCode(401, serviceFailedResponse);
            }

            ServiceResponse<GenToken> response = new ServiceResponse<GenToken>
            {
                Data = CreateToken(user),
                Message = "success",
                Success = true
            };



            return Ok(response);
        }











        #region NonAction-Method

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
            //Generate-AccessToken-jwt
            CreateJwt(user, out JwtSecurityTokenHandler tokenHandler, out SecurityToken token);


            //Generate-RefreshToken
             //Log this output paramter referesh token in the immemory object
            CreateRefreshToken(user, token, out string createToken, out RefereshTokenModel refershToken);

            //set refereshtoken on cookie header response
            SetRefreshToken(refershToken,createToken);


            
            return new GenToken
            {
                Token = tokenHandler.WriteToken(token),
                RefreshToken = createToken,
                Username = user.Username,
                Role = user.Role,
                ValidTo = DateTime.Now.Add(_projectOptions.TokenLifeTime),
                ValidFrom = token.ValidFrom

            };

        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        private void CreateJwt(User user, out JwtSecurityTokenHandler tokenHandler, out SecurityToken token)
        {
            tokenHandler = new JwtSecurityTokenHandler();
            var claims = new ClaimsIdentity(new[]
                         {
                new Claim(ClaimTypes.Name,user.Username),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()), //to identify the refereshtoken id

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
            token = tokenHandler.CreateToken(tokenDescriptor);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        private static void CreateRefreshToken(User user, SecurityToken token, out string createToken, out RefereshTokenModel refershToken)
        {
            //createToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            createToken = RandomString(100);
            refershToken = new RefereshTokenModel()
            {
                JwtId = token.Id,
                IsUsed = false,
                IsRevoked = false,
                UserId = user.Id,
                DateCreated = DateTime.UtcNow,
                DateExpired = DateTime.UtcNow.AddMonths(3),
                Token = SHA512Converter.GenerateSHA512String(createToken)

            };
            //create RefreshToken and log it
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public void SetRefreshToken(RefereshTokenModel refereshTokenModel,string refereshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = refereshTokenModel.DateExpired
            };
            Response.Cookies.Append("refreshToken",refereshToken,cookieOptions);
            user.RefreshToken = refereshToken;
            user.TokenCreated = refereshTokenModel.DateCreated;
            user.TokenExpired = refereshTokenModel.DateExpired;
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        private static string RandomString(int length)
        {
            var random = new Random();
            var chars = "ABCjkDEFGH1JKLMNOPQYRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz" + Guid.NewGuid().ToString().ToUpper();
            return new string(Enumerable.Repeat(chars, length).Select(x => x[random.Next(length)]).ToArray());
        }

        #endregion
    }
}
