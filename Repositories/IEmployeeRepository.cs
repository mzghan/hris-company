using HRIS.Api.Models;

namespace HRIS.Api.Repositories;

public interface IEmployeeRepository
{
    Task<List<Employee>> GetAllAsync();
    Task<Employee?> GetByIdAsync(int id);
    Task<Employee?> GetByNumberAsync(string employeeNumber);
    Task<bool> WorkEmailExistsAsync(string email);
    Task<Employee> AddAsync(Employee employee);
    Task UpdateAsync(Employee employee);
    Task DeleteAsync(Employee employee);

    // Dipakai saat generate rantai approval leave request: atasan langsung
    // aktif (Hierarchy tipe Direct Manager), lalu atasannya atasan, dst.
    Task<List<Employee>> GetManagerChainAsync(int employeeId, int maxLevels);

    // Dipakai KPI (manager hanya boleh menilai bawahan langsungnya).
    Task<bool> IsDirectManagerOfAsync(int managerId, int employeeId);

    // Dasar claim "IsManager" saat login.
    Task<bool> HasActiveSubordinatesAsync(int employeeId);

    // Riwayat Employment/Hierarchy: baris lama ditutup + baris baru dibuat
    // dalam satu transaction. Baris "current" yang dikirim harus hasil dari
    // method Get...Async di bawah (tracked), dan EndDate-nya sudah diisi Service.
    Task<EmployeeEmployment?> GetCurrentEmploymentAsync(int employeeId);
    Task ReplaceEmploymentAsync(EmployeeEmployment? current, EmployeeEmployment next);
    Task<EmployeeHierarchy?> GetActiveDirectManagerRowAsync(int employeeId);
    Task ReplaceDirectManagerAsync(EmployeeHierarchy? current, EmployeeHierarchy? next);
}
