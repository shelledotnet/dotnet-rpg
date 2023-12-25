using AutoMapper;
using dotnet_rpg.AttributeUsed;
using dotnet_rpg.domain.Dtos;
using dotnet_rpg.domain.Services;
using dotnet_rpg.Extensions;
using dotnet_rpg.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace dotnet_rpg.Controllers
{
    [Produces("application/json", "application/xml")]  //output formatter Media type: Accept header
    [Consumes("application/json")] //input-formatter Media type: content-type header
    [Route("api/character/")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ServiceFailedResponse))]
    
    
    #if DEBUG
    public class CharacterController : ControllerBase
    {
        
        private readonly ICharacterService _characterService;
        private readonly IMapper _mapper;
        private readonly IPropertyMappingService propertyMappingService;
        private readonly ILogger<CharacterController> _logger;
        public CharacterController(ILogger<CharacterController> logger, ICharacterService characterService,
            IMapper mapper, IPropertyMappingService propertyMappingService)
        {
            _characterService = characterService;
            _mapper = mapper;
            this.propertyMappingService = propertyMappingService;
            _logger = logger;   
        }

        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ServiceForbidenResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ServiceFailedResponse))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ServiceResponse<List<GetCharacterDto>>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ServiceBadResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ServiceFailedResponse))]
        [HttpGet("GetAll"),Authorize(Roles ="admin,user")]
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


        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ServiceForbidenResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ServiceFailedResponse))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ServiceResponse<PagedList<Employee>>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ServiceBadResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ServiceFailedResponse))]
        [HttpGet("GetAllByFilters",Name = "GetAllByFilters"), AllowAnonymous]
        public async Task<IActionResult> GetAllByFilters([FromQuery]FiltersByState model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetApiResponse());
            }
            if (!propertyMappingService.ValidMappingExistsFor<Employee, EmployeeResponseDto>(model.orderBy))
            {
                return BadRequest(new  { responseId = DateTime.Now.ToString("yyyyMMddHHmmssfffffff"), success = false , message = "invalid key for order value"});

            }
            try
            {
                ServiceResponse<List<EmployeeResponseDto>> genericResponse = new();
                _logger.LogInformation("info");
                PagedList<Employee> result = await _characterService.GetStateByName(model);

                 
                if(result.Count > 0)
                {
                  var previousPageLink =  result.HasPrevious ? CreateAuthourUrl(model,ResourceUriType.PreviousPage) : null;

                  var nextPageLink = result.HasNext ? CreateAuthourUrl(model, ResourceUriType.NextPage) : null;


                    var paginationMetadata = new
                    {
                        totalCount = result.TotalCount,
                        pageSize = result.PageSize,
                        currentPage = result.CurrentPage,
                        totalPage = result.TotalPages,
                        previousPageLink = previousPageLink,
                        nextPageLink = nextPageLink
                    };


                    Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));


                    genericResponse.PaginationParas = paginationMetadata;
                    genericResponse.Data = _mapper.Map<List<EmployeeResponseDto>>(result);
                    genericResponse.Success = true;
                    genericResponse.Message = "sucess";
                    return Ok(genericResponse);
                }
                return  NotFound(_mapper.Map<ServiceFailedResponse>(genericResponse));

            }
            catch (Exception ex)
            {

                _logger.LogError($"{ex}");
                ServiceFailedResponse serviceResponse = new() { Success = false, Message = ex.InnerException?.Message != null ? ex.InnerException.Message : ex.Message };
                return StatusCode(500, serviceResponse);
            }
        }


        public string? CreateAuthourUrl(FiltersByState filtersByState,ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetAllByFilters",
                        new
                        {
                            orderBy = filtersByState.orderBy,
                            pageNumber = filtersByState.pageNumber - 1,
                            pageSize = filtersByState.pageSize,
                            mainCategory = filtersByState?.mainCategory,
                            searchQuery = filtersByState?.searchQuery

                        });
                case ResourceUriType.NextPage:
                    return Url.Link("GetAllByFilters",
                        new
                        {
                            orderBy = filtersByState.orderBy,
                            pageNumber = filtersByState.pageNumber + 1,
                            pageSize = filtersByState.pageSize,
                            mainCategory = filtersByState.mainCategory,
                            searchQuery = filtersByState.searchQuery


                        });
                default:
                    return Url.Link("GetAllByFilters",
                        new
                        {
                            orderBy = filtersByState.orderBy,
                            pageNumber = filtersByState.pageNumber,
                            pageSize = filtersByState.pageSize,
                            mainCategory = filtersByState.mainCategory,
                            searchQuery = filtersByState.searchQuery


                        });
            }
        }


        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ServiceForbidenResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ServiceFailedResponse))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ServiceResponse<List<EmployeeResponseDto>>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ServiceBadResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ServiceFailedResponse))]
        [HttpGet("SalaryByGender"), AllowAnonymous]
        public async Task<IActionResult> SalaryByGender([FromQuery] SalaryByGender salaryByGender)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetApiResponse());
            }
            try
            {
                _logger.LogInformation("info");
                ServiceResponse<List<EmployeeResponseDto>> genericResponse = await _characterService.MyQuery(salaryByGender.gender);
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



        [ProducesResponseType(StatusCodes.Status406NotAcceptable, Type = typeof(ServiceMethodNotAailabeResponse))]
        [ProducesResponseType(StatusCodes.Status405MethodNotAllowed, Type = typeof(ServiceMethodNotAailabeResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ServiceFailedResponse))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ServiceResponse<GetCharacterDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest ,Type = typeof(ServiceBadResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ServiceFailedResponse))]
        [HttpGet("GetSingle",Name = "GetSingle"),AllowAnonymous]
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
        [HttpDelete,AllowAnonymous]
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
