using HRIS.Api.Common;
using HRIS.Api.Models;
using HRIS.Api.DTOs.Manpower;
namespace HRIS.Api.Services;
public interface IManpowerService
{
    Task<List<ManpowerRequestResponseDto>> GetAsync(UserContext actor);
    Task<ManpowerRequestResponseDto> CreateAsync(ManpowerRequestCreateDto dto, UserContext actor);
    Task<List<ManpowerVacancy>> GetVacanciesAsync(UserContext actor);
    Task AddFilingAsync(int id, string fileType, Microsoft.AspNetCore.Http.IFormFile file, UserContext actor);
}
