using HRIS.Api.Models;

namespace HRIS.Api.Repositories;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task<bool> UsernameExistsAsync(string username);
    Task<User> AddAsync(User user);
    Task<bool> AnyUserExistsAsync();
}
