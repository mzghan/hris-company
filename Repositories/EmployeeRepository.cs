using HRIS.Api.Data;
using HRIS.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HRIS.Api.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly AppDbContext _context;

    public EmployeeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Employee>> GetAllAsync() =>
        await _context.Employees
            .Include(e => e.Department)
            .Include(e => e.Manager)
            .ToListAsync();

    public async Task<Employee?> GetByIdAsync(int id) =>
        await _context.Employees
            .Include(e => e.Department)
            .Include(e => e.Manager)
            .FirstOrDefaultAsync(e => e.Id == id);

    public async Task<Employee?> GetByEmailAsync(string email) =>
        await _context.Employees.FirstOrDefaultAsync(e => e.Email == email);

    public async Task<Employee> AddAsync(Employee employee)
    {
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();
        return employee;
    }

    public async Task UpdateAsync(Employee employee)
    {
        _context.Employees.Update(employee);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Employee employee)
    {
        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Employee>> GetManagerChainAsync(int employeeId, int maxLevels)
    {
        var chain = new List<Employee>();
        var current = await GetByIdAsync(employeeId);

        while (current?.ManagerId is not null && chain.Count < maxLevels)
        {
            var manager = await GetByIdAsync(current.ManagerId.Value);
            if (manager is null) break;

            chain.Add(manager);
            current = manager;
        }

        return chain;
    }

    public async Task<List<Employee>> GetActiveEmployeesAsync() =>
        await _context.Employees
            .Where(e => e.IsActive)
            .ToListAsync();
}
