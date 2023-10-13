using AutoMapper;
using dotnet_rpg.domain.Dtos;
using dotnet_rpg.domain.Models;
using Microsoft.Extensions.Logging;
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
        private static readonly List<Character> characters = new()
        {

            new Character(){ Class=RpgClass.Mage.ToString(),Id=1, Defense=23, HitPoints=89, Intelligence=23, Name="Winger", Strength=56 },
            new Character(){ Class=RpgClass.Cleric.ToString(), Id=2,Defense=23, HitPoints=98,Intelligence=12,Name="Pjamx",Strength=87 }
        };
        private readonly ILogger<CharacterServices> _logger;
        private readonly IMapper _mapper;

        public CharacterServices(ILogger<CharacterServices> logger,IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
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
                    response.Message = "success";


                }
                else
                {
                    response.Success = false;
                    response.Message = "character doesn't exist";


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
    }
}
