using HRIS.Api.Models;

namespace HRIS.Api.Repositories;

public interface IEmployeeRepository
{
    Task<List<Employee>> GetAllAsync();
    Task<Employee?> GetByIdAsync(int id);
    Task<Employee?> GetByEmailAsync(string email);
    Task<Employee> AddAsync(Employee employee);
    Task UpdateAsync(Employee employee);
    Task DeleteAsync(Employee employee);

    // Dipakai saat generate rantai approval leave request:
    // ambil manager langsung, lalu manager-nya-manager, dst.
    Task<List<Employee>> GetManagerChainAsync(int employeeId, int maxLevels);

    // Dipakai saat generate PayrollItem: hanya employee aktif yang
    // dimasukkan ke PayrollPeriod baru.
    Task<List<Employee>> GetActiveEmployeesAsync();
}
