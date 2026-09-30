using HRIS.Api.Models;

namespace HRIS.Api.Repositories;

public interface IRegulationRepository
{
    Task<List<Regulation>> GetAllAsync(bool includeInactive = false);
    Task<Regulation?> GetByIdAsync(int id);
    Task<Regulation> AddAsync(Regulation regulation);
    Task UpdateAsync(Regulation regulation);
}
