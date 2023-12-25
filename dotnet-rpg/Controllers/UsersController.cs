using AutoMapper;
using dotnet_rpg.AttributeUsed;
using dotnet_rpg.domain.Services;
using dotnet_rpg.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Serilog;

namespace dotnet_rpg.Controllers
{
    [Produces("application/json", "application/xml")]  //output formatter Media type: Accept header
    [Consumes("application/json")] //input-formatter Media type: content-type header
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ServiceFailedResponse))]
    [TypeFilter(typeof(ApiKeyAttribute))]
    public class UsersController : ControllerBase
    {
        private readonly ProjectOptions _projectOptions;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UsersController(IOptionsMonitor<ProjectOptions> projectOptions,IUserRepository userRepository, IMapper mapper)
        {
            _projectOptions = projectOptions.CurrentValue;
            _userRepository = userRepository;
            _mapper = mapper;
        }


        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ServiceForbidenResponse))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ServiceResponse<GenTokens>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ServiceBadResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ServiceFailedResponse))]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetApiResponse());
            }
            try
            {
                Log.Information("info");
                
                ServiceResponse<GenTokens> genericResponse = await _userRepository.Authenticate(loginRequestDto);
                return genericResponse.Success == true ? Ok(genericResponse)
                                                       : Unauthorized(_mapper.Map<ServiceFailedResponse>(genericResponse));

            }
            catch (Exception ex)
            {

                Log.Error($"{ex}");
                ServiceFailedResponse serviceResponse = new() { Success = false, Message = ex.InnerException?.Message != null ? ex.InnerException.Message : ex.Message };
                return StatusCode(500, serviceResponse);
            }
        }

        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ServiceForbidenResponse))]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ServiceResponse<LoginResponseDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ServiceBadResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ServiceFailedResponse))]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetApiResponse());
            }
            try
            {
                Log.Information("info");

                ServiceResponse<LoginResponseDto> genericResponse = await _userRepository.Register(registerRequestDto);
                return genericResponse.Success == true ? CreatedAtRoute("",genericResponse)
                                                       : Conflict(_mapper.Map<ServiceFailedResponse>(genericResponse));

            }
            catch (Exception ex)
            {

                Log.Error($"{ex}");
                ServiceFailedResponse serviceResponse = new() { Success = false, Message = ex.InnerException?.Message != null ? ex.InnerException.Message : ex.Message };
                return StatusCode(500, serviceResponse);
            }
        }


        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ServiceForbidenResponse))]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ServiceResponse<LoginResponseDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ServiceBadResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ServiceFailedResponse))]
        [HttpPost("~/api/users/referesh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto refreshTokenRequestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetApiResponse());
            }
            try
            {
                Log.Information("info");

                ServiceResponse<GenTokens> genericResponse = await _userRepository.RefreshToken(refreshTokenRequestDto);
                return genericResponse.Success == true ? CreatedAtRoute("", genericResponse)
                                                       : Conflict(_mapper.Map<ServiceFailedResponse>(genericResponse));

            }
            catch (Exception ex)
            {

                Log.Error($"{ex}");
                ServiceFailedResponse serviceResponse = new() { Success = false, Message = ex.InnerException?.Message != null ? ex.InnerException.Message : ex.Message };
                return StatusCode(500, serviceResponse);
            }
        }




    }
}
