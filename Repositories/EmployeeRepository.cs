using HRIS.Api.Common;
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

    // Seluruh riwayat Employment & Hierarchy ikut di-load; baris "terkini"
    // dipilih di Service (EndDate null). Jumlah baris riwayat per karyawan kecil.
    private IQueryable<Employee> BaseQuery() =>
        _context.Employees
            .Include(e => e.NationalityCountry)
            .Include(e => e.Gender)
            .Include(e => e.Families).ThenInclude(f => f.Relationship)
            .Include(e => e.Families).ThenInclude(f => f.Gender)
            .Include(e => e.Families).ThenInclude(f => f.Relationship)
            .Include(e => e.Families).ThenInclude(f => f.Gender)
            .Include(e => e.Contacts).ThenInclude(c => c.ContactType)
            .Include(e => e.Employments).ThenInclude(m => m.EmploymentType)
            .Include(e => e.Employments).ThenInclude(m => m.EmploymentStatus)
            .Include(e => e.Employments).ThenInclude(m => m.Vendor)
            .Include(e => e.Employments).ThenInclude(m => m.Organization)
            .Include(e => e.Employments).ThenInclude(m => m.Location)
            .Include(e => e.Employments).ThenInclude(m => m.JobLevel)
            .Include(e => e.Employments).ThenInclude(m => m.JobTitle)
            .Include(e => e.Employments).ThenInclude(m => m.Grade)
            .Include(e => e.Managers).ThenInclude(h => h.Manager)
            .Include(e => e.Managers).ThenInclude(h => h.HierarchyType)
            .AsSplitQuery();

    public async Task<List<Employee>> GetAllAsync() =>
        await BaseQuery().OrderBy(e => e.EmployeeNumber).ToListAsync();

    public async Task<Employee?> GetByIdAsync(int id) =>
        await BaseQuery().FirstOrDefaultAsync(e => e.Id == id);

    public async Task<Employee?> GetByNumberAsync(string employeeNumber) =>
        await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeNumber == employeeNumber);

    public async Task<bool> WorkEmailExistsAsync(string email) =>
        await _context.EmployeeContacts.AnyAsync(c =>
            c.ContactType!.ContactTypeName == RefNames.WorkEmail && c.ContactValue == email);

    public async Task<Employee> AddAsync(Employee employee)
    {
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();
        return employee;
    }

    public async Task UpdateAsync(Employee employee)
    {
        // Entity sudah tracked (dari GetByIdAsync). Update() akan menandai seluruh graph
        // (Employment, Contact, Hierarchy) Modified dan mengisi updated_at semuanya.
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
        var visited = new HashSet<int> { employeeId };
        var currentId = employeeId;

        while (chain.Count < maxLevels)
        {
            var managerId = await _context.EmployeeHierarchies
                .Where(h => h.EmployeeId == currentId
                            && h.EndDate == null
                            && h.HierarchyType!.HierarchyTypeName == RefNames.DirectManager)
                .Select(h => (int?)h.ManagerId)
                .FirstOrDefaultAsync();

            // visited mencegah loop tak berujung kalau data hierarchy sempat siklik.
            if (managerId is null || !visited.Add(managerId.Value)) break;

            var manager = await _context.Employees.FirstOrDefaultAsync(e => e.Id == managerId.Value);
            if (manager is null) break;

            chain.Add(manager);
            currentId = manager.Id;
        }

        return chain;
    }

    public async Task<bool> IsDirectManagerOfAsync(int managerId, int employeeId) =>
        await _context.EmployeeHierarchies.AnyAsync(h =>
            h.ManagerId == managerId
            && h.EmployeeId == employeeId
            && h.EndDate == null
            && h.HierarchyType!.HierarchyTypeName == RefNames.DirectManager);

    public async Task<bool> HasActiveSubordinatesAsync(int employeeId) =>
        await _context.EmployeeHierarchies.AnyAsync(h =>
            h.ManagerId == employeeId
            && h.EndDate == null
            && h.HierarchyType!.HierarchyTypeName == RefNames.DirectManager
            && h.Employee!.IsActive);

    public async Task<EmployeeEmployment?> GetCurrentEmploymentAsync(int employeeId) =>
        await _context.EmployeeEmployments
            .FirstOrDefaultAsync(m => m.EmployeeId == employeeId && m.EndDate == null);

    public async Task ReplaceEmploymentAsync(EmployeeEmployment? current, EmployeeEmployment next)
    {
        // Dua SaveChanges terpisah (bukan satu): unique index parsial "satu baris
        // terkini per employee" bisa bentrok kalau insert baris baru dieksekusi
        // sebelum update penutupan baris lama. Transaction menjaga keduanya atomik.
        var ownTransaction = _context.Database.CurrentTransaction is null;
        await using var transaction = ownTransaction ? await _context.Database.BeginTransactionAsync() : null;

        if (current is not null)
            await _context.SaveChangesAsync();

        _context.EmployeeEmployments.Add(next);
        await _context.SaveChangesAsync();

        if (transaction is not null)
            await transaction.CommitAsync();
    }

    public async Task<EmployeeHierarchy?> GetActiveDirectManagerRowAsync(int employeeId) =>
        await _context.EmployeeHierarchies
            .FirstOrDefaultAsync(h => h.EmployeeId == employeeId
                                      && h.EndDate == null
                                      && h.HierarchyType!.HierarchyTypeName == RefNames.DirectManager);

    public async Task ReplaceDirectManagerAsync(EmployeeHierarchy? current, EmployeeHierarchy? next)
    {
        var ownTransaction = _context.Database.CurrentTransaction is null;
        await using var transaction = ownTransaction ? await _context.Database.BeginTransactionAsync() : null;

        if (current is not null)
            await _context.SaveChangesAsync();

        if (next is not null)
        {
            _context.EmployeeHierarchies.Add(next);
            await _context.SaveChangesAsync();
        }

        if (transaction is not null)
            await transaction.CommitAsync();
    }
}
