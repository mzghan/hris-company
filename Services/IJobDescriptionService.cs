using HRIS.Api.Common;
using HRIS.Api.DTOs.JobDescription;
namespace HRIS.Api.Services;
public interface IJobDescriptionService
{
    Task<List<JobDescriptionResponseDto>> GetAsync(UserContext actor);
    Task<JobDescriptionResponseDto> CreateAsync(JobDescriptionCreateDto dto, UserContext actor, bool submit);
    Task<JobDescriptionResponseDto> GetByIdAsync(int id, UserContext actor);
    Task<JobDescriptionResponseDto> SignJobHolderAsync(int id, string signature, UserContext actor);
    Task<JobDescriptionResponseDto> SignManagerAsync(int id, string signature, UserContext actor);
}
