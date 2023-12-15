using dotnet_rpg.domain.Dtos;
using dotnet_rpg.domain.Models;

namespace dotnet_rpg.domain.Services
{
    public interface ICharacterService
    {
        Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacter();

        Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int Id);
        Task<PagedList<Employee>> GetStateByName(FiltersByState filtersByState);
        Task<ServiceResponse<GetCharacterDto>> AddCharacter(AddCharacterDto addcharacter);

        Task<ServiceResponse<GetCharacterDto>> DeleteCharacter(int Id);

        Task<ServiceResponse<GetCharacterDto>> UpdateCharacterById(UpdateCharacterDto updateCharacterDto);

        Task<ServiceResponse<List<EmployeeResponseDto>>> MyQuery(string gender);
    }
}
