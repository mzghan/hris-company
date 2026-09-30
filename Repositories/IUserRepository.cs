using HRIS.Api.Models;

namespace HRIS.Api.Repositories;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task<bool> UsernameExistsAsync(string username);
    Task<bool> EmployeeHasAccountAsync(int employeeId);
    Task<User> AddAsync(User user);
    Task<bool> AnyUserExistsAsync();
    Task UpdateLastLoginAsync(User user);
    Task<List<Role>> GetRolesByNamesAsync(IEnumerable<string> roleNames);
}
