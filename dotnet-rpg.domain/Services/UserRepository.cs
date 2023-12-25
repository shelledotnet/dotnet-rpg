using AutoMapper;
using dotnet_rpg.domain.Data;
using dotnet_rpg.domain.Dtos;
using dotnet_rpg.domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Sockets;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace dotnet_rpg.domain.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbContextFactory<EmployeeManagerDbContext> contextFactory;
        private readonly IMapper _mapper;
        private readonly ProjectOptions _projectOptions;
        private readonly TokenValidationParameters _tokenValidationParameters;

        public UserRepository(IDbContextFactory<EmployeeManagerDbContext> contextFactory, IMapper mapper,
            IOptionsMonitor<ProjectOptions> projectOptions, TokenValidationParameters tokenValidationParameters)
        {
            this.contextFactory = contextFactory;
            _mapper = mapper;
            _projectOptions = projectOptions.CurrentValue;
            _tokenValidationParameters = tokenValidationParameters;
        }

        //C:\Users\Mohammed.Shelle\AppData\Roaming\Microsoft\UserSecrets\c1d2563a-74d4-4d85-a9fe-13820fd35c88\secrets.json
        public async Task<ServiceResponse<GenTokens>> Authenticate(LoginRequestDto loginRequestDto)
        {
            EmployeeManagerDbContext context = await contextFactory.CreateDbContextAsync();
            ServiceResponse<GenTokens> response = new();
            try
            {
                UsersDto usersDto = _mapper.Map<UsersDto>(loginRequestDto);

                Users? user = await context.Users.FirstOrDefaultAsync(u => u.Username.ToLower() == usersDto.Username.Trim().ToLower() && u.Active ); 
                if (user is  null)
                {
                    response.Success = false;
                    response.Message = _projectOptions.NotFound;
                    return response;
                }
                bool isPasswordatched = MatchPasswordHash(usersDto.Password, user.Password, user.PasswordKey);
                if (isPasswordatched)
                {

                    //Generate-AccessToken-jwt
                    CreateJWT(user,context, out JwtSecurityTokenHandler tokenHandler, out SecurityToken token);


                    RemovedUnusedRefeshToken(user,context);


                    //Generate-RefreshToken
                    //Log this output paramter referesh token in the immemory object
                    CreateRefreshToken(user, token,context, out string createToken, out RefereshTokenModels refershToken);

                    //set refereshtoken on cookie header response
                    //SetRefreshToken(refershToken, createToken);



                    GenTokens genTokens = new()
                    {
                        Token = tokenHandler.WriteToken(token),
                        RefreshToken = createToken,
                        Username = user.Username,
                        ValidTo = token.ValidTo,//DateTime.Now.Add(_projectOptions.TokenLifeTime),
                        ValidFrom = token.ValidFrom,
                        RefreshTokenExpireDate = refershToken.DateExpired,
                        Email = user.Email,
                        Role = GetUserRoles(user)

                    };

                    response.Data = genTokens;
                    response.Success = true;
                    response.Message = "success";
                }
                else if (!isPasswordatched)
                {
                    response.Success = false;
                    response.Message = _projectOptions.NotFound;
                }


            }
            catch (Exception ex)
            {
          
                string message = $"{ex}";
                Log.Error(message);
                response.Success = false;
                response.Message = ex.InnerException?.Message != null ? ex.InnerException.Message : ex.Message;
            }
            return response;
        }

        private bool MatchPasswordHash(string? password, byte[] passwordHash, byte[] passwordKey)
        {
            using var hmac = new HMACSHA512(passwordKey);
            byte[] computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(passwordHash);
        }

        public async Task<ServiceResponse<LoginResponseDto>> Register(RegisterRequestDto registerRequestDto)
        {
            EmployeeManagerDbContext? context = await contextFactory.CreateDbContextAsync();
            ServiceResponse<LoginResponseDto> response = new();
            try
            {

                bool isAnyUserActive = await UserAlreadyExists(registerRequestDto.Username);
                if (isAnyUserActive)
                {
                    response.Success = false;
                    response.Message = _projectOptions.Conflict;
                    return response;
                }


                Users users = _mapper.Map<Users>(registerRequestDto);


                using var hmac = new HMACSHA512();
                users.PasswordKey = hmac.Key;
                users.Password = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(registerRequestDto.Password));
                users.Active = true;

                context.Users.Add(users);
                int result = await context.SaveChangesAsync();
                if (result == 1)
                {
                    response.Data = new LoginResponseDto { Username = registerRequestDto.Username, Token = "not availabe for now" };
                    response.Success = true;
                    response.Message = _projectOptions.Ok;
                }
                else if (result == 0)
                {
                    response.Success = false;
                    response.Message = _projectOptions.NotFound;
                }


            }
            catch (Exception ex)
            {

                string message = $"{ex}";
                Log.Error(message);
                response.Success = false;
                response.Message = ex.InnerException?.Message != null ? ex.InnerException.Message : ex.Message;
            }
            return response;

        }


        public async Task<ServiceResponse<GenTokens>> RefreshToken(RefreshTokenRequestDto refreshTokenRequestDto)
        {
            EmployeeManagerDbContext? context = await contextFactory.CreateDbContextAsync();
            ServiceResponse<GenTokens> response = new();
            try
            {

                ClaimsPrincipal validatedToken = GetPrincipalFromToken(refreshTokenRequestDto.Token);

                if (validatedToken == null)
                {
                    response.Success = false;
                    response.Message = _projectOptions.InValidToken;
                    return response;
                }
                long expireDateUnix = long.Parse(validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

                DateTime expireDate = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)
                                             .AddSeconds(expireDateUnix);


                if (expireDate > DateTime.UtcNow)
                {
                    response.Success = false;
                    response.Message = _projectOptions.ValidToken;
                    return response;
                }
                var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
                var storedRefershToken = await context.RefereshTokens.SingleOrDefaultAsync(x => x.Token == 
                SHA512Converter.GenerateSHA512String(refreshTokenRequestDto.RefreshToken));
                if (storedRefershToken == null)
                {
                    response.Success = false;
                    response.Message = _projectOptions.FalseToken;
                    return response;
                }
                if (DateTime.UtcNow > storedRefershToken.DateExpired)
                {
                    response.Success = false;
                    response.Message = _projectOptions.TokenExpired;
                    return response;
                }
                if (storedRefershToken.IsRevoked)
                {
                    response.Success = false;
                    response.Message = _projectOptions.TokenRevoked;
                    return response;
                }
                if (storedRefershToken.IsUsed)
                {
                    response.Success = false;
                    response.Message = _projectOptions.TokenUsed;
                    return response;
                }
                if (storedRefershToken.JwtId != jti)
                {
                    response.Success = false;
                    response.Message = _projectOptions.TokenNotMatched;
                    return response;
                }

                storedRefershToken.IsUsed = true;
                context.RefereshTokens.Update(storedRefershToken);
                await context.SaveChangesAsync();

                var userid = validatedToken.Claims.Single(x => x.Type == "Id").Value;
                //var user = await customersDbContext.FindByIdAsync(validatedToken.Claims.Single(x => x.Type == "Id").Value);ClaimTypes.NameIdentifier

                var user = await context.Users.FindAsync(Convert.ToInt32(userid));

                //Generate-AccessToken-jwt
                CreateJWT(user, context, out JwtSecurityTokenHandler tokenHandler, out SecurityToken token);

                RemovedUnusedRefeshToken(user, context);


                //Generate-RefreshToken
                //Log this output paramter referesh token in the immemory object
                CreateRefreshToken(user, token, context, out string createToken, out RefereshTokenModels refershToken);

                //set refereshtoken on cookie header response
                //SetRefreshToken(refershToken, createToken);



                GenTokens genTokens = new()
                {
                    Token = tokenHandler.WriteToken(token),
                    RefreshToken = createToken,
                    Username = user.Username,
                    ValidTo = token.ValidTo,//DateTime.Now.Add(_projectOptions.TokenLifeTime),
                    ValidFrom = token.ValidFrom,
                    RefreshTokenExpireDate = refershToken.DateExpired,
                    Email = user.Email,
                    Role = GetUserRoles(user)

                };

                response.Data = genTokens;
                response.Success = true;
                response.Message = "success";


            }
            catch (Exception ex)
            {

                string message = $"{ex}";
                Log.Error(message);
                response.Success = false;
                response.Message = ex.InnerException?.Message != null ? ex.InnerException.Message : ex.Message;
            }
            return response;

        }





        public async Task<bool> UserAlreadyExists(string username)
        {
            try
            {
                var context = await contextFactory.CreateDbContextAsync();
                return await context.Users.AnyAsync(u => u.Username.ToLower() == username.Trim().ToLower());

            }
            catch (Exception ex)
            {
                string message = $"{ex}";
                Log.Error(message);
                return false;
            }
        }


        private  void CreateJWT(Users users, EmployeeManagerDbContext context, out JwtSecurityTokenHandler tokenHandler, out SecurityToken token)
        {
             tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Convert.FromBase64String(_projectOptions.SecreteKey);  //convert to byte[] from base64

            ClaimsIdentity? claimsIdentity = new ClaimsIdentity(new[] {
                //endeavour not to use sensitive data  pwd

               // new Claim(ClaimTypes.NameIdentifier, users.Id.ToString()),
                new Claim("Id",users.Id.ToString()),
                new Claim("IsBlocked", users.Blocked.ToString()),
                new Claim("IsActive", users.Active.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub,users.Email),
                new Claim(JwtRegisteredClaimNames.Email,users.Email),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()), //to identify the refereshtoken id
                new Claim("UserName",users.Username ?? "")

            });

            foreach (var item in GetUserRole(users.Id,context))
            {
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, item.Name));
            }



            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);
            //HmacSha256Signature  the bigger the number the longer the key character length

            SecurityTokenDescriptor tokenDescriptor = new()
            {
                #region CustomeClaims
                Subject = claimsIdentity,
                #endregion


                #region GenericClaims
                Issuer = _projectOptions.Issuer,
                Audience = _projectOptions.Audience,
                Expires = DateTime.Now.Add(_projectOptions.TokenLifeTime),
                SigningCredentials = signingCredentials,
                #endregion

            };
            token = tokenHandler.CreateToken(tokenDescriptor);
           
        }


        private static void CreateRefreshToken(Users user, SecurityToken token, EmployeeManagerDbContext context, out string createToken, out RefereshTokenModels refershToken)
        {
            //createToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            createToken = RandomString(100);
            refershToken = new RefereshTokenModels()
            {
                JwtId = token.Id,
                IsUsed = false,
                IsRevoked = false,
                UsersId = user.Id,
                DateCreated = DateTime.UtcNow,
                DateExpired = DateTime.UtcNow.AddMonths(3),
                Token = SHA512Converter.GenerateSHA512String(createToken)
                

            };

            //create RefreshToken and log it
            context.RefereshTokens.Add(refershToken);
            context.SaveChanges();
        }

        private List<Role> GetUserRole(int UserId, EmployeeManagerDbContext context)
        {
            try
            {
                List<Role> rolesMasters = (from UM in context.Users
                                              join UR in context.Role on UM.Id equals UR.UsersId
                                              join RM in context.Role on UR.Id equals RM.Id
                                              where UM.Id == UserId
                                              select RM).ToList();
                return rolesMasters;
            }
            catch (Exception ex)
            {
                string message = $"{ex}";
                Log.Error(message);
               
                return new List<Role>();
            }
        }

        private void RemovedUnusedRefeshToken(Users user, EmployeeManagerDbContext context)
        {
            var refreshToken = context.RefereshTokens.Where(req => req.UsersId == user.Id && req.IsRevoked == false && req.IsUsed == false).FirstOrDefault();
            if (refreshToken != null)
            {

                context.RefereshTokens.Remove(refreshToken);
                context.SaveChangesAsync();

            }

        }


        public List<string> GetUserRoles(Users user)
        {
            List<string> roles = new List<string>();
            foreach (var item in user.Roles)
            {
                roles.Add(item.Name.ToLower());
            }
            return roles;
        }


        //this validate token b4 using it to get referesh token
        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                _tokenValidationParameters.ValidateLifetime = false;
                var principal = tokenHandler.ValidateToken(token, _tokenValidationParameters, out var validatedToken);
                if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
                {
                    return null;
                }
                return principal;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        //confirm the return type algorithm used to generate jwt
        public bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
        {
            return (validatedToken is JwtSecurityToken jwtSecurityToken) &&
                jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
        }


        private static string RandomString(int length)
        {
            var random = new Random();
            var chars = "ABCjkDEFGH1JKLMNOPQYRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz" + Guid.NewGuid().ToString().ToUpper();
            return new string(Enumerable.Repeat(chars, length).Select(x => x[random.Next(length)]).ToArray());
        }
    }
}
