using HRIS.Api.Models;

namespace HRIS.Api.Repositories;

public interface IEmployeeSalaryRepository
{
    Task<List<EmployeeSalary>> GetByEmployeeAsync(int employeeId);

    // Baris EmployeeSalary yang berlaku pada tanggal asOf: EffectiveDate
    // terbesar yang masih <= asOf. Null berarti employee belum punya
    // data gaji sama sekali pada tanggal itu.
    Task<EmployeeSalary?> GetActiveAsync(int employeeId, DateOnly asOf);

    Task<EmployeeSalary> AddAsync(EmployeeSalary salary);
}
