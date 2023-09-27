using AutoMapper;
using dotnet_rpg.domain.Dtos;

namespace dotnet_rpg.Profiles
{
    public class CharacterProfile : Profile
    {
        public CharacterProfile()
        {
            #region Mapping Left to right

            #endregion  YouVerifyV2CacModel
            CreateMap<Character, GetCharacterDto>();
            CreateMap<AddCharacterDto, Character>();

        }
    }
}
