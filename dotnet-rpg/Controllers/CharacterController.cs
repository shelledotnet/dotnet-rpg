using dotnet_rpg.domain.Models;
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
        
        private static readonly List<Character> Knight = new()
        {

            new Character(){ Class=RpgClass.Mage,Id=1, Defense=23, HitPoints=89, Intelligence=23, Name="Winger", Strength=56 },
            new Character(){ Class=RpgClass.Cleric, Id=2,Defense=23, HitPoints=98,Intelligence=12,Name="Pjamx",Strength=87 }
        };
        private readonly ILogger<CharacterController> _logger;

        public CharacterController(ILogger<CharacterController> logger)
        {
            _logger = logger;

        }

        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(GenericFailed))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GenericResponse<Character>))]
        [HttpGet("GetAll")]
        public IActionResult Get()
        {
            try
            {
                _logger.LogInformation("info");
                if (Knight.Count > 0)
                {
                    GenericResponse<List<Character>> response = new();



                    response.Data = Knight;
                    response.Code = "00";
                    response.Description = "success";

                    return Ok(response);
                }
                else
                {
                   

                    return NotFound(new GenericFailed { Code = "99", Description = "failed" });

                }
            }
            catch (Exception)
            {

                throw;
            }
        }


        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(GenericFailed))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GenericResponse<Character>))]
        [HttpGet]
        public IActionResult GetSingle([FromQuery]CharacterModel characterModel)
        {
            
            try
            {
                _logger.LogInformation("info");
                Character? character = Knight.Find(x => x.Id == characterModel.Id);
                if (character is not null)
                {
                    GenericResponse<Character> response = new();

                    response.Data = character;
                    response.Code = "00";
                    response.Description = "success";

                    return Ok(response);
                }
                else
                {
                   
                    return NotFound(new GenericFailed {Code="99",Description="failed"});
                }
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
