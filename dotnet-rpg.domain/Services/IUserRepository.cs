using dotnet_rpg.domain.Dtos;
using dotnet_rpg.domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotnet_rpg.domain.Services
{
    public interface IUserRepository
    {
        Task<ServiceResponse<GenTokens>> Authenticate(LoginRequestDto loginRequestDto);

        Task<ServiceResponse<LoginResponseDto>> Register(RegisterRequestDto registerRequestDto);

        Task<ServiceResponse<GenTokens>> RefreshToken(RefreshTokenRequestDto refreshTokenRequestDto);

        Task<bool> UserAlreadyExists(string username);
    }
}
