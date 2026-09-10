using HRIS.Api.Models;

namespace HRIS.Api.Repositories;

public interface IKpiPeriodRepository
{
    Task<List<KpiPeriod>> GetAllAsync();
    Task<KpiPeriod?> GetByIdAsync(int id);
    Task<KpiPeriod?> GetByNameYearAsync(string name, int year);
    Task<KpiPeriod> AddAsync(KpiPeriod period);
    Task UpdateAsync(KpiPeriod period);
}
