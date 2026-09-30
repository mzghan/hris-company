using HRIS.Api.DTOs.Auth;

namespace HRIS.Api.Services;

public interface IAuthService
{
    // actorIsSupport: hanya akun Support yang boleh memberikan role Support.
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto, bool actorIsSupport);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
}
