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
            CreateMap<Users, UsersDto>().ReverseMap();
            CreateMap<Users, LoginResponseDto>();
            CreateMap<RegisterRequestDto, Users>()
                .ForMember(dest => dest.Password, act => act.Ignore());
            CreateMap<ServiceResponse<List<GetCharacterDto>>, ServiceFailedResponse>();
            CreateMap<ServiceResponse<PagedList<Employee>>, ServiceFailedResponse>();
            CreateMap<ServiceResponse<GetCharacterDto>, ServiceFailedResponse>();
            CreateMap<ServiceResponse<LoginResponseDto>, ServiceFailedResponse>();
            CreateMap<ServiceResponse<GenTokens>, ServiceFailedResponse>();
            CreateMap<LoginRequestDto, UsersDto>();
            CreateMap<Employee, EmployeeResponseDto>()
                 .ForMember(
                            dest => dest.Name,
                            opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                 .ForMember(
                            dest => dest.Age,
                            opt => opt.MapFrom(src => CalculateAge(src.Dob)));
            CreateMap<Department, DepartmentDto>();

            CreateMap<int?, int>().ConvertUsing((src, dest) => src ?? dest);
            CreateMap<UpdateCharacterDto, Character>()
                     .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
           //Provide Mapping Dept from Department Property
           //     .ForMember(dest => dest.Dept, act => act.MapFrom(src => src.Department));
        }
        public  int CalculateAge(DateTime? dateOfBirth)
        {
            int age = 0;
            age = DateTime.Now.Year - dateOfBirth.Value.Year;
            if (DateTime.Now.DayOfYear < dateOfBirth.Value.DayOfYear)
                age--;

            return age;
        }
    }
}
