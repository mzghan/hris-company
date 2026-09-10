using HRIS.Api.Models;

namespace HRIS.Api.Repositories;

public interface IKpiCriteriaRepository
{
    Task<List<KpiCriteria>> GetAllAsync();
    Task<KpiCriteria?> GetByIdAsync(int id);
    Task<KpiCriteria> AddAsync(KpiCriteria criteria);
    Task UpdateAsync(KpiCriteria criteria);
    Task DeleteAsync(KpiCriteria criteria);
}
