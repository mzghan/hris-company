using HRIS.Api.Models;
namespace HRIS.Api.Repositories;
public interface IManpowerRepository
{
    Task<List<ManpowerRequest>> GetAllAsync(int? employeeId=null);
    Task<ManpowerRequest?> GetByIdAsync(int id);
    Task<ManpowerRequest> AddAsync(ManpowerRequest x);
    Task UpdateAsync(ManpowerRequest x);
}
