using HRIS.Api.Models;
namespace HRIS.Api.Repositories;
public interface IJobDescriptionRepository
{
    Task<List<JobDescription>> GetAllAsync();
    Task<JobDescription?> GetByIdAsync(int id);
    Task<JobDescription> AddAsync(JobDescription x);
    Task UpdateAsync(JobDescription x);
    Task AddBankAsync(JobDescriptionBank x);
    Task UpsertMasterAsync(JobDescriptionMaster x);
}
