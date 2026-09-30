using HRIS.Api.Data;
using HRIS.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HRIS.Api.Repositories;

public class OrganizationRepository : IOrganizationRepository
{
    private readonly AppDbContext _context;

    public OrganizationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Organization>> GetAllAsync() =>
        await _context.Organizations
            .Include(o => o.Parent)
            .OrderBy(o => o.Path)
            .ToListAsync();

    public async Task<Organization?> GetByIdAsync(int id) =>
        await _context.Organizations
            .Include(o => o.Parent)
            .Include(o => o.Children)
            .FirstOrDefaultAsync(o => o.Id == id);

    public async Task<List<Organization>> GetDescendantsAsync(string path) =>
        await _context.Organizations
            .Where(o => o.Path.StartsWith(path) && o.Path != path)
            .ToListAsync();

    public async Task<Dictionary<int, int>> GetEmployeeCountsAsync() =>
        await _context.EmployeeEmployments
            .Where(m => m.EndDate == null)
            .GroupBy(m => m.OrganizationId)
            .Select(g => new { OrganizationId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.OrganizationId, x => x.Count);

    public async Task<bool> HasEmploymentsAsync(int organizationId) =>
        await _context.EmployeeEmployments.AnyAsync(m => m.OrganizationId == organizationId);

    public async Task<Organization> AddAsync(Organization organization)
    {
        _context.Organizations.Add(organization);
        await _context.SaveChangesAsync();
        return organization;
    }

    public async Task UpdateAsync(Organization organization)
    {
        _context.Organizations.Update(organization);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateRangeAsync(IEnumerable<Organization> organizations)
    {
        _context.Organizations.UpdateRange(organizations);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Organization organization)
    {
        _context.Organizations.Remove(organization);
        await _context.SaveChangesAsync();
    }
}
