using HRIS.Api.Models;

namespace HRIS.Api.Services;

public interface IJwtService
{
    (string token, DateTime expiresAt) GenerateToken(User user);
}
