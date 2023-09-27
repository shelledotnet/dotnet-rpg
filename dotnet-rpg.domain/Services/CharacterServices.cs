using AutoMapper;
using dotnet_rpg.domain.Dtos;
using dotnet_rpg.domain.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            try
            {
                ServiceResponse<GetCharacterDto> response = new();



                _logger.LogInformation("info");
                Character character = _mapper.Map<Character>(addcharacter);
                character.Id = characters.Max(c => c.Id)+ 1;
                characters.Add(character);
                GetCharacterDto getCharacterDto = _mapper.Map<GetCharacterDto>(character);
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
                return response;



            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacter()
        {
            ServiceResponse<List<GetCharacterDto>> response = new();
            try
            {
                _logger.LogInformation("info");
                List<GetCharacterDto> getCharacter = _mapper.Map<List<GetCharacterDto>>(characters);
                if (characters.Count > 0)
                {

                    response.Data = getCharacter;
                    response.Success = true;
                    response.Message = "success";


                }
                else
                {
                    response.Success = false;
                    response.Message = "character doesn't exist";


                }
                return response;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int Id)
        {
            try
            {
                ServiceResponse<GetCharacterDto> response = new();
                _logger.LogInformation("info");
                Character? character = characters.Find(x => x.Id == Id);
                GetCharacterDto getCharacterDto = _mapper.Map<GetCharacterDto>(character);
                if (character is not null)
                {
                    
                        response.Data = getCharacterDto;
                        response.Success = true;
                        response.Message = "success";

                }
                else
                {

                    response.Success = false;
                    response.Message = $"character with id number {Id} doesn't exist";
                }
                return response;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
