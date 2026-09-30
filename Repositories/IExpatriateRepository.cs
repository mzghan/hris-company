using HRIS.Api.Models;

namespace HRIS.Api.Repositories;

public interface IExpatriateRepository
{
    Task<List<Employee>> GetExpatriatesAsync();
    Task<Employee?> GetEmployeeAsync(int employeeId);
    Task<EmployeeIdentity?> GetIdentityAsync(int id);
    Task<EmployeeIdentity> AddIdentityAsync(EmployeeIdentity identity);
    Task UpdateIdentityAsync(EmployeeIdentity identity);
    Task DeleteIdentityAsync(EmployeeIdentity identity);
    Task<bool> IdentityNumberExistsAsync(int employeeId, int identityTypeId, string identityNumber, int? exceptId = null);
}
