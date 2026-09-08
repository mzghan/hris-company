using HRIS.Api.Models;

namespace HRIS.Api.Repositories;

public interface IDepartmentRepository
{
    Task<List<Department>> GetAllAsync();
    Task<Department?> GetByIdAsync(int id);
    Task<Department> AddAsync(Department department);
    Task UpdateAsync(Department department);
    Task DeleteAsync(Department department);
}
