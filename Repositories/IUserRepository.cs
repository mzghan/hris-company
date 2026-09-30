using HRIS.Api.Models;

namespace HRIS.Api.Repositories;

// Nama + email kerja user, untuk notifikasi.
public record UserContact(int UserId, string Name, string? Email);

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task<bool> UsernameExistsAsync(string username);
    Task<bool> EmployeeHasAccountAsync(int employeeId);
    Task<User> AddAsync(User user);
    Task<bool> AnyUserExistsAsync();
    Task UpdateLastLoginAsync(User user);
    Task<List<Role>> GetRolesByNamesAsync(IEnumerable<string> roleNames);

    // Dipakai approval engine & notifikasi untuk mengubah "approver" (Employee/Role) menjadi user penerima.
    Task<int?> GetUserIdByEmployeeIdAsync(int employeeId);
    Task<List<int>> GetUserIdsByRoleIdAsync(int roleId);
    Task<List<UserContact>> GetContactsAsync(IEnumerable<int> userIds);
}
