using HRIS.Api.Common;
using HRIS.Api.DTOs.Manpower;
namespace HRIS.Api.Services;
public interface IManpowerService
{
    Task<List<ManpowerRequestResponseDto>> GetAsync(UserContext actor);
    Task<ManpowerRequestResponseDto> CreateAsync(ManpowerRequestCreateDto dto, UserContext actor);
}
