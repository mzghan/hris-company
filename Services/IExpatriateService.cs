using HRIS.Api.Common;
using HRIS.Api.DTOs.Expatriate;

namespace HRIS.Api.Services;

public interface IExpatriateService
{
    Task<List<ExpatriateEmployeeResponseDto>> GetExpatriatesAsync(UserContext actor);
    Task<ExpatriateEmployeeResponseDto> GetAsync(int employeeId, UserContext actor);
    Task<EmployeeIdentityResponseDto> AddIdentityAsync(int employeeId, EmployeeIdentityCreateDto dto, UserContext actor);
    Task<EmployeeIdentityResponseDto> UpdateIdentityAsync(int id, EmployeeIdentityCreateDto dto, UserContext actor);
    Task DeleteIdentityAsync(int id, UserContext actor);
}
