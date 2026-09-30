using HRIS.Api.Common;
using HRIS.Api.Data;
using HRIS.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HRIS.Api.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByUsernameAsync(string username) =>
        await _context.Users
            .Include(u => u.Employee)
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Username == username);

    public async Task<bool> UsernameExistsAsync(string username) =>
        await _context.Users.AnyAsync(u => u.Username == username);

    public async Task<bool> EmployeeHasAccountAsync(int employeeId) =>
        await _context.Users.AnyAsync(u => u.EmployeeId == employeeId);

    public async Task<User> AddAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<bool> AnyUserExistsAsync() => await _context.Users.AnyAsync();

    public async Task UpdateLastLoginAsync(User user)
    {
        user.LastLoginAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task<List<Role>> GetRolesByNamesAsync(IEnumerable<string> roleNames)
    {
        var names = roleNames.ToList();
        return await _context.Roles.Where(r => names.Contains(r.RoleName)).ToListAsync();
    }

    public async Task<int?> GetUserIdByEmployeeIdAsync(int employeeId) =>
        await _context.Users
            .Where(u => u.EmployeeId == employeeId && u.IsActive)
            .Select(u => (int?)u.Id)
            .FirstOrDefaultAsync();

    public async Task<List<int>> GetUserIdsByRoleIdAsync(int roleId) =>
        await _context.UserRoles
            .Where(ur => ur.RoleId == roleId && ur.User!.IsActive)
            .Select(ur => ur.UserId)
            .ToListAsync();

    public async Task<List<UserContact>> GetContactsAsync(IEnumerable<int> userIds)
    {
        var ids = userIds.ToList();
        var users = await _context.Users
            .AsNoTracking()
            .Include(u => u.Employee)
            .Where(u => ids.Contains(u.Id) && u.IsActive)
            .ToListAsync();

        var employeeIds = users.Where(u => u.EmployeeId != null).Select(u => u.EmployeeId!.Value).ToList();
        var emails = await _context.EmployeeContacts
            .AsNoTracking()
            .Where(c => employeeIds.Contains(c.EmployeeId) && c.ContactType!.ContactTypeName == RefNames.WorkEmail)
            .Select(c => new { c.EmployeeId, c.ContactValue })
            .ToListAsync();

        return users
            .Select(u => new UserContact(
                u.Id,
                u.Employee?.FullName ?? u.Username,
                u.EmployeeId is null ? null : emails.FirstOrDefault(e => e.EmployeeId == u.EmployeeId)?.ContactValue))
            .ToList();
    }
}
