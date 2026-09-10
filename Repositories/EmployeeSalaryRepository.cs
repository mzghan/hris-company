using HRIS.Api.Data;
using HRIS.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HRIS.Api.Repositories;

public class EmployeeSalaryRepository : IEmployeeSalaryRepository
{
    private readonly AppDbContext _context;

    public EmployeeSalaryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<EmployeeSalary>> GetByEmployeeAsync(int employeeId) =>
        await _context.EmployeeSalaries
            .Where(s => s.EmployeeId == employeeId)
            .OrderByDescending(s => s.EffectiveDate)
            .ToListAsync();

    public async Task<EmployeeSalary?> GetActiveAsync(int employeeId, DateOnly asOf) =>
        await _context.EmployeeSalaries
            .Where(s => s.EmployeeId == employeeId && s.EffectiveDate <= asOf)
            .OrderByDescending(s => s.EffectiveDate)
            .FirstOrDefaultAsync();

    public async Task<EmployeeSalary> AddAsync(EmployeeSalary salary)
    {
        _context.EmployeeSalaries.Add(salary);
        await _context.SaveChangesAsync();
        return salary;
    }
}
