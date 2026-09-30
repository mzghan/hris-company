using HRIS.Api.Common;
using HRIS.Api.DTOs.Learning;

namespace HRIS.Api.Services;

public interface ILearningMaterialService
{
    Task<List<LearningMaterialResponseDto>> GetAllAsync();
    Task<LearningMaterialResponseDto> CreateAsync(LearningMaterialCreateDto dto, UserContext actor);
    Task<LearningMaterialResponseDto> UpdateAsync(int id, LearningMaterialCreateDto dto, UserContext actor);
    Task DeleteAsync(int id, UserContext actor);
}
