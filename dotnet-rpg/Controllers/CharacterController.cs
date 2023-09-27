using AutoMapper;
using dotnet_rpg.domain.Dtos;
using dotnet_rpg.domain.Models;
using dotnet_rpg.domain.Services;
using dotnet_rpg.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_rpg.Controllers
{
    [Produces("application/json")]
    [Consumes("application/json")]
    [Route("api/character/")]
    [ApiController]

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

        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ServiceResponse))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ServiceResponse<List<GetCharacterDto>>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ServiceBadResponse))]
        [HttpGet("GetAll")]
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
                return genericResponse.Success == true ? Ok(genericResponse) : NotFound(new ServiceResponse { Success = genericResponse.Success, Message = genericResponse.Message});
          
            }
            catch (Exception)
            {

                throw;
            }
        }


        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ServiceResponse))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ServiceResponse<GetCharacterDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest ,Type = typeof(ServiceBadResponse))]
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
                return genericResponse.Success == true ? Ok(genericResponse) : NotFound(new ServiceResponse { Success = genericResponse.Success, Message = genericResponse.Message });

            }
            catch (Exception)
            {

                throw;
            }
        }


        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ServiceBadResponse))]
        [HttpPost]
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
                return genericResponse.Success == true ? CreatedAtRoute(nameof(GetSingle), new { id = genericResponse.Data?.Id }, genericResponse) : BadRequest(new ServiceResponse { Success = genericResponse.Success, Message = genericResponse.Message });

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
    #else
    #endif

}
