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
            CreateMap<ServiceResponse<GetCharacterDto>, ServiceFailedResponse>();
            CreateMap<int?, int>().ConvertUsing((src, dest) => src ?? dest);
            CreateMap<UpdateCharacterDto, Character>()
                     .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
          
        }
    }
}
