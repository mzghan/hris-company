using HRIS.Api.Data;
using HRIS.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HRIS.Api.Repositories;

public class ExpatriateRepository : IExpatriateRepository
{
    private readonly AppDbContext _context;
    public ExpatriateRepository(AppDbContext context) => _context = context;

    private IQueryable<Employee> ExpatQuery() => _context.Employees
        .Include(e => e.NationalityCountry)
        .Include(e => e.Identities).ThenInclude(i => i.IdentityType)
        .Include(e => e.Contacts).ThenInclude(c => c.ContactType)
        .Where(e => e.IsActive && e.NationalityCountry != null && e.NationalityCountry.CountryCode != "ID");

    public Task<List<Employee>> GetExpatriatesAsync() => ExpatQuery()
        .OrderBy(e => e.FullName).ToListAsync();

    public Task<Employee?> GetEmployeeAsync(int employeeId) => ExpatQuery()
        .FirstOrDefaultAsync(e => e.Id == employeeId);

    public Task<EmployeeIdentity?> GetIdentityAsync(int id) => _context.EmployeeIdentities
        .Include(i => i.Employee).Include(i => i.IdentityType)
        .FirstOrDefaultAsync(i => i.Id == id);

    public async Task<EmployeeIdentity> AddIdentityAsync(EmployeeIdentity identity)
    {
        _context.EmployeeIdentities.Add(identity);
        await _context.SaveChangesAsync();
        return identity;
    }

    public async Task UpdateIdentityAsync(EmployeeIdentity identity) => await _context.SaveChangesAsync();

    public async Task DeleteIdentityAsync(EmployeeIdentity identity)
    {
        _context.EmployeeIdentities.Remove(identity);
        await _context.SaveChangesAsync();
    }

    public Task<bool> IdentityNumberExistsAsync(int employeeId, int identityTypeId, string identityNumber, int? exceptId = null) =>
        _context.EmployeeIdentities.AnyAsync(i =>
            i.EmployeeId == employeeId && i.IdentityTypeId == identityTypeId &&
            i.IdentityNumber == identityNumber && (exceptId == null || i.Id != exceptId.Value));
}
