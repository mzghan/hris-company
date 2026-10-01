using HRIS.Api.Models;
namespace HRIS.Api.Repositories;
public interface IPersonalActionRepository
{
    Task<List<PersonalAction>> GetAllAsync(int? employeeId);
    Task<PersonalAction?> GetByIdAsync(int id);
    Task<PersonalAction> AddAsync(PersonalAction x);
    Task UpdateAsync(PersonalAction x);
    Task<Employee?> GetEmployeeAsync(int id);
    Task<EmployeeEmployment?> GetCurrentEmploymentAsync(int employeeId);
    Task<EmployeeHierarchy?> GetCurrentDirectManagerAsync(int employeeId);
    Task ApplyApprovedAsync(PersonalAction action);
}