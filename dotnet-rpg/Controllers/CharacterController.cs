using AutoMapper;
using dotnet_rpg.AttributeUsed;
using dotnet_rpg.domain.Dtos;
using dotnet_rpg.domain.Services;
using dotnet_rpg.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_rpg.Controllers
{
    [Produces("application/json")]
    [Consumes("application/json")]
    [Route("api/character/")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ServiceFailedResponse))]

#if DEBUG
    public class CharacterController : ControllerBase
    {
        
        private readonly ICharacterService _characterService;
        private readonly IMapper _mapper;
        private readonly ILogger<CharacterController> _logger;
        public CharacterController(ILogger<CharacterController> logger, ICharacterService characterService,IMapper mapper)
        {
            _characterService = characterService;
            _mapper = mapper;
            _logger = logger;   
        }

        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ServiceFailedResponse))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ServiceResponse<List<GetCharacterDto>>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ServiceBadResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ServiceFailedResponse))]
        [HttpGet("GetAll"),Authorize(Roles ="Admin")]
        public async Task<IActionResult> Get()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetApiResponse());
            }
            try
            {
                _logger.LogInformation("info");
                ServiceResponse<List<GetCharacterDto>> genericResponse =await _characterService.GetAllCharacter();
                return genericResponse.Success == true ? Ok(genericResponse)
                                                       : NotFound(_mapper.Map<ServiceFailedResponse>(genericResponse));

            }
            catch (Exception ex)
            {

                _logger.LogError($"{ex}");
                ServiceFailedResponse serviceResponse = new() { Success = false, Message = ex.InnerException?.Message != null ? ex.InnerException.Message : ex.Message };
                return StatusCode(500, serviceResponse);
            }
        }


        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ServiceFailedResponse))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ServiceResponse<GetCharacterDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest ,Type = typeof(ServiceBadResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ServiceFailedResponse))]
        [HttpGet("GetSingle",Name = "GetSingle")]
        public async Task<IActionResult> GetSingle([FromQuery]CharacterModel characterModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetApiResponse());
            }
            try
            {
                _logger.LogInformation("info");
                ServiceResponse<GetCharacterDto> genericResponse =await _characterService.GetCharacterById(characterModel.Id);
                return genericResponse.Success == true ? Ok(genericResponse)
                                                       : NotFound(_mapper.Map<ServiceFailedResponse>(genericResponse));

            }
            catch (Exception ex)
            {

                _logger.LogError($"{ex}");
                ServiceFailedResponse serviceResponse = new() { Success = false, Message = ex.InnerException?.Message != null ? ex.InnerException.Message : ex.Message };
                return StatusCode(500, serviceResponse);
            }
        }


        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ServiceResponse<GetCharacterDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ServiceBadResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ServiceFailedResponse))]
        [HttpPost,AllowAnonymous]
        public async Task<IActionResult>  AddCharacter([FromBody] AddCharacterDto addcharacter)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetApiResponse());
            }


            try
            {
                _logger.LogInformation("info");
                ServiceResponse<GetCharacterDto> genericResponse =await _characterService.AddCharacter(addcharacter);
                return genericResponse.Success == true ? CreatedAtRoute(nameof(GetSingle), new { id = genericResponse.Data?.Id }, genericResponse) 
                                                       : BadRequest(_mapper.Map<ServiceFailedResponse>(genericResponse));
            }
            catch (Exception ex)
            {

                _logger.LogError($"{ex}");
                ServiceFailedResponse serviceResponse = new() { Success = false, Message = ex.InnerException?.Message != null ? ex.InnerException.Message : ex.Message };
                return StatusCode(500, serviceResponse);
            }
        }
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ServiceFailedResponse))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ServiceResponse<GetCharacterDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ServiceBadResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ServiceFailedResponse))]
        [HttpPut,AllowAnonymous]
        public async Task<IActionResult> UpdateCharacter([FromBody] UpdateCharacterDto updatecharacter)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetApiResponse());
            }


            try
            {
                _logger.LogInformation("info");
                ServiceResponse<GetCharacterDto> genericResponse = await _characterService.UpdateCharacterById(updatecharacter);
                return genericResponse.Success == true ? Ok(genericResponse)
                                                       : NotFound(_mapper.Map<ServiceFailedResponse>(genericResponse));

            }
            catch (Exception ex)
            {

                _logger.LogError($"{ex}");
                ServiceFailedResponse serviceResponse = new() { Success = false, Message = ex.InnerException?.Message != null ? ex.InnerException.Message : ex.Message };
                return StatusCode(500, serviceResponse);
            }
        }

        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ServiceFailedResponse))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ServiceResponse<GetCharacterDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ServiceBadResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ServiceFailedResponse))]
        [HttpDelete]
        public async Task<IActionResult> DeleteCharacter([FromQuery] DeleteCharacterDto deleteCharacterDto)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetApiResponse());
            }


            try
            {
                _logger.LogInformation("info");
                ServiceResponse<GetCharacterDto> genericResponse = await _characterService.DeleteCharacter(deleteCharacterDto.Id);
                return genericResponse.Success == true ? Ok(genericResponse)
                                                       : NotFound(_mapper.Map<ServiceFailedResponse>(genericResponse));

            }
            catch (Exception ex)
            {

                _logger.LogError($"{ex}");
                ServiceFailedResponse serviceResponse = new() { Success = false, Message = ex.InnerException?.Message != null ? ex.InnerException.Message : ex.Message };
                return StatusCode(500, serviceResponse);
            }
        }



    }
    #else
    #endif

}
