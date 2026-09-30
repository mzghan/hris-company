using HRIS.Api.Models;

namespace HRIS.Api.Repositories;

public interface ILearningMaterialRepository
{
    Task<List<LearningMaterial>> GetAllAsync();
    Task<LearningMaterial?> GetByIdAsync(int id);
    Task<LearningMaterial> AddAsync(LearningMaterial material);
    Task UpdateAsync(LearningMaterial material);
}
