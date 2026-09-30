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
}
