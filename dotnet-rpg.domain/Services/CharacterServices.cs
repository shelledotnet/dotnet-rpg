using AutoMapper;
using dotnet_rpg.domain.Data;
using dotnet_rpg.domain.Dtos;
using dotnet_rpg.domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Formatting = Newtonsoft.Json.Formatting;

namespace dotnet_rpg.domain.Services
{
    public class CharacterServices : ICharacterService
    {
        #region In-memory-Object
        private static readonly List<Character> characters = new()
        {
            new Character(){ Class=RpgClass.Mage.ToString(),Id=1,State="Kwara",Gender="Male" ,Defense=23, HitPoints=89, Intelligence=23, Name="Winger", Strength=56,DOB="2001-11-23"},
            new Character(){ Class=RpgClass.Cleric.ToString(), Id=2,State="Kwara",Gender="FeMale",Defense=23, HitPoints=98,Intelligence=12,Name="Pjamx",Strength=87,DOB="1988-10-23" },
            new Character(){ Class=RpgClass.Mage.ToString(),Id=3,State="Oyo",Gender="FeMale" ,Defense=23, HitPoints=89, Intelligence=23, Name="Winger", Strength=56,DOB="2001-11-23"},
            new Character(){ Class=RpgClass.Cleric.ToString(), Id=4,State="Oyo",Gender="Male",Defense=23, HitPoints=98,Intelligence=12,Name="Pjamx",Strength=87,DOB="1988-10-23" },
            new Character(){ Class=RpgClass.Mage.ToString(),Id=5,State="Lagos",Gender="Male" ,Defense=23, HitPoints=89, Intelligence=23, Name="Winger", Strength=56,DOB="2001-11-23"},
            new Character(){ Class=RpgClass.Cleric.ToString(), Id=2,State="Ogun",Gender="FeMale",Defense=23, HitPoints=98,Intelligence=12,Name="Pjamx",Strength=87,DOB="1988-10-23" },
            new Character(){ Class=RpgClass.Mage.ToString(),Id=6,State="Lagos",Gender="Male" ,Defense=23, HitPoints=89, Intelligence=23, Name="Winger", Strength=56,DOB="2001-11-23"},
            new Character(){ Class=RpgClass.Cleric.ToString(), Id=7,State="Ogun",Gender="FeMale",Defense=23, HitPoints=98,Intelligence=12,Name="Pjamx",Strength=87,DOB="1988-10-23" },
            new Character(){ Class=RpgClass.Mage.ToString(),Id=8,State="Lagos",Gender="Male" ,Defense=23, HitPoints=89, Intelligence=23, Name="Winger", Strength=56,DOB="2001-11-23"},
            new Character(){ Class=RpgClass.Cleric.ToString(), Id=9,State="Lagos",Gender="Male",Defense=23, HitPoints=98,Intelligence=12,Name="Pjamx",Strength=87,DOB="1988-10-23" }
        }; 
        #endregion
        private readonly ILogger<CharacterServices> _logger;
        private readonly IMapper _mapper;
        private readonly ProjectOptions _projectOptions;
        private readonly IDbContextFactory<EmployeeManagerDbContext> contextFactory;
        private readonly IPropertyMappingService _propertyMappingService;

        public CharacterServices(ILogger<CharacterServices> logger,IMapper mapper, 
            IOptionsMonitor<ProjectOptions> projectOptions, IDbContextFactory<EmployeeManagerDbContext> contextFactory,
            IPropertyMappingService propertyMappingService)
        {
            _logger = logger;
            _mapper = mapper;
            _projectOptions = projectOptions.CurrentValue;
            this.contextFactory = contextFactory;
            _propertyMappingService = propertyMappingService;
        }


        public async Task<ServiceResponse<GetCharacterDto>> AddCharacter(AddCharacterDto addcharacter)
        {
            ServiceResponse<GetCharacterDto> response = new();
            GetCharacterDto getCharacterDto = new();
            try
            {
                



                _logger.LogInformation("info");
                Character character = _mapper.Map<Character>(addcharacter);
                character.Id = characters.Max(c => c.Id)+ 1;
                characters.Add(character);
                getCharacterDto = _mapper.Map<GetCharacterDto>(character);
                if (character is not null)
                {

                    response.Data = getCharacterDto;
                    response.Success = true;
                    response.Message = "success";


                }
                else
                {
                    response.Success = false;
                    response.Message = "fail";


                }

            }
            catch (Exception ex)
            {
                string message = $"{ex}";
                _logger.LogError(message);
                response.Success = false;
                response.Message = ex.InnerException?.Message != null ? ex.InnerException.Message : ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacter()
        {
            ServiceResponse<List<GetCharacterDto>> response = new();
            List<GetCharacterDto> getCharacter = new();
            try
            {
                _logger.LogInformation("info");
                if (characters.Count > 0)
                {
                    getCharacter = _mapper.Map<List<GetCharacterDto>>(characters);


                    response.Data = getCharacter;
                    response.Success = true;
                    response.Message = _projectOptions.Ok;


                }
                else
                {
                    response.Success = false;
                    response.Message = _projectOptions.NotFound;


                }
            }
            catch (Exception ex)
            {
                string message = $"{ex}";
                _logger.LogError(message);
                response.Success = false;
                response.Message = ex.InnerException?.Message != null ? ex.InnerException.Message : ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int Id)
        {
            ServiceResponse<GetCharacterDto> response = new();
            GetCharacterDto getCharacterDto = new();
            Character? character = new();
            try
            {
                

                _logger.LogInformation("info");
                character = characters.Find(x => x.Id == Id);
                if (character is not null)
                {
                    
                        response.Data = _mapper.Map<GetCharacterDto>(character);
                        response.Success = true;
                        response.Message = "success";

                }
                else
                {

                    response.Success = false;
                    response.Message = $"character with id number {Id} doesn't exist";
                }
            }
            catch (Exception ex)
            {
                string message = $"{ex}";
                _logger.LogError(message);
                response.Success = false;
                response.Message = ex.InnerException?.Message != null ? ex.InnerException.Message : ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacterById(UpdateCharacterDto updateCharacterDto)
        {
            ServiceResponse<GetCharacterDto> response = new();
            GetCharacterDto getCharacterDto = new();
            Character? character = new();
            try
            {

                _logger.LogInformation("info");

                character = characters.Find(x => x.Id == updateCharacterDto.Id);
                if(character is null)
                {
                    response.Success = false;
                    response.Message = $"character with id number {updateCharacterDto.Id} doesn't exist";
                }
                else
                {

                   
                    //mapping from left to right
                    _mapper.Map(updateCharacterDto, character);

                    #region Candidateof AutoMapper
                    //character.Id = updateCharacterDto.Id;
                    //character.Name = updateCharacterDto.Name;
                    //character.Strength = updateCharacterDto.Strength;
                    //character.Defense = updateCharacterDto.Defense;
                    //character.HitPoints = updateCharacterDto.HitPoints;
                    //character.Class = updateCharacterDto.Class;
                    //character.Intelligence = updateCharacterDto.Intelligence;


                    #endregion

                    response.Data    = _mapper.Map<GetCharacterDto>(character);
                    response.Success = true;
                    response.Message = "success";
                }

               




            }
            catch (Exception ex)
            {
                string message = $"{ex}";
                _logger.LogError(message);
                response.Success = false;
                response.Message = ex.InnerException?.Message != null ? ex.InnerException.Message : ex.Message;
                
            }

            return response;
        }

        public async Task<ServiceResponse<GetCharacterDto>> DeleteCharacter(int Id)
        {
            ServiceResponse<GetCharacterDto> response = new();
            GetCharacterDto getCharacterDto = new();
            Character? character = new();
            try
            {

                _logger.LogInformation("info");

                character = characters.Find(x => x.Id == Id);
                if (character is null)
                {
                    response.Success = false;
                    response.Message = $"character with id number {Id} doesn't exist";
                }
                else
                {
                    characters.Remove(character);

                    response.Data = _mapper.Map<GetCharacterDto>(character);
                    response.Success = true;
                    response.Message = $"character with an id {Id} was deleted successfully";
                }

                




            }
            catch (Exception ex)
            {
                string message = $"{ex}";
                _logger.LogError(message);
                response.Success = false;
                response.Message = ex.InnerException?.Message != null ? ex.InnerException.Message : ex.Message;
                
            }

            return response;
        }

        public async Task<PagedList<Employee>> GetStateByName(FiltersByState model)
        {
           
           
            try
            {
            

                _logger.LogInformation("info");
                var context = await contextFactory.CreateDbContextAsync();

                //this IQueryable is  for defered execution making filetering and seaching easier and faster to be executed at .ToListAsync();
                var collections = context.Employees as IQueryable<Employee>;



                if (!string.IsNullOrEmpty(model.mainCategory) && collections.Any())
                {

                    collections = collections?.Where(a => a.State.ToLower() == model.mainCategory.Trim().ToLower())
                                              .Include(a => a.Department);


                }

                if (!string.IsNullOrEmpty(model.searchQuery) && collections.Any())
                {

                    model.searchQuery = model.searchQuery.Trim().ToLower();

                    collections = collections?.Where(a => a.State.ToLower().Contains(model.searchQuery)
                                                                     || a.FirstName.Contains(model.searchQuery)
                                                                     || a.LastName.Contains(model.searchQuery)
                                                                     || a.Gender.Contains(model.searchQuery)
                                                                     )
                                            .Include(a => a.Department);
                }
                if (!string.IsNullOrEmpty(model.orderBy))
                {
                    
                    model.orderBy = model.orderBy.Trim().ToLower();

                    //get property mapping dictionary
                    var authorPropertyMappingDictionary = _propertyMappingService.GetPropertyMapping<Employee, EmployeeResponseDto>();

                    collections = collections.ApplySort(model.orderBy, authorPropertyMappingDictionary);
                }


                if (collections.Any())
                {


                    return await PagedList<Employee>.CreateAsync(collections.Include(a => a.Department),
                                                                           model.pageNumber, model.pageSize);

                    //getCharacter = _mapper.Map<List<EmployeeResponseDto>>(await PagedList<Employee>.CreateAsync(collections.Include(a => a.Department), 
                                                                        //   model.pageNumber, model.pageSize));

                    
                }
                return null;
                //else if(!collections.Any())
                // {
                //     response.Success = false;
                //     response.Message = _projectOptions.NotFound;

                // }
               

            }
            catch (Exception ex)
            {
                string message = $"{ex}";
                _logger.LogError(message);
                return null;
                //response.Success = false;
                //response.Message = ex.InnerException?.Message != null ? ex.InnerException.Message : ex.Message;
            }
            //return response;
        }

       

        public async Task<ServiceResponse<List<EmployeeResponseDto>>> MyQuery(string gender)
        {

            ServiceResponse<List<EmployeeResponseDto>> response = new();
            List<EmployeeResponseDto> getCharacter = new();
            try
            {
                var context = await contextFactory.CreateDbContextAsync();
                var empList = context.Employees;
                var avgSalary = empList.Select(e => e.Salary).Average();
                var emp = empList
                    .Where(e => e.Gender.ToLower() == gender.ToLower())
                    .Take(3)
                    .Where(e => e.Salary > avgSalary);
                    //.OrderBy(e => e.Age);

                if (emp.Any())
                {
                    getCharacter = _mapper.Map<List<EmployeeResponseDto>>(await emp.Include(a => a.Department)
                                                                                    .ToListAsync());


                    response.Data = getCharacter;
                    response.Success = true;
                    response.Message = _projectOptions.Ok;
                }
                else if (!emp.Any())
                {
                    response.Success = false;
                    response.Message = _projectOptions.NotFound;
                }


            }
            catch (Exception ex)
            {

                string message = $"{ex}";
                _logger.LogError(message);
                response.Success = false;
                response.Message = ex.InnerException?.Message != null ? ex.InnerException.Message : ex.Message;
            }
            return response;
        }
    }
}
