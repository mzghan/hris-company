using HRIS.Api.DTOs.Auth;
using HRIS.Api.Exceptions;
using HRIS.Api.Models;
using HRIS.Api.Models.Enums;
using HRIS.Api.Repositories;

namespace HRIS.Api.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IJwtService _jwtService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUserRepository userRepository,
        IEmployeeRepository employeeRepository,
        IJwtService jwtService,
        ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _employeeRepository = employeeRepository;
        _jwtService = jwtService;
        _logger = logger;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        if (await _userRepository.UsernameExistsAsync(dto.Username))
            throw new BadRequestException($"Username '{dto.Username}' sudah dipakai.");

        if (dto.Role != UserRole.Admin)
        {
            if (dto.EmployeeId is null)
                throw new BadRequestException("EmployeeId wajib diisi untuk role Employee/Manager.");

            var employee = await _employeeRepository.GetByIdAsync(dto.EmployeeId.Value)
                ?? throw new BadRequestException($"EmployeeId {dto.EmployeeId} tidak ditemukan.");
        }

        var user = new User
        {
            Username = dto.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = dto.Role,
            EmployeeId = dto.EmployeeId
        };

        await _userRepository.AddAsync(user);
        _logger.LogInformation("User {Username} terdaftar dengan role {Role}", user.Username, user.Role);

        return BuildResponse(user);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _userRepository.GetByUsernameAsync(dto.Username)
            ?? throw new BadRequestException("Username atau password salah.");

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            throw new BadRequestException("Username atau password salah.");

        return BuildResponse(user);
    }

    private AuthResponseDto BuildResponse(User user)
    {
        var (token, expiresAt) = _jwtService.GenerateToken(user);
        return new AuthResponseDto
        {
            Token = token,
            UserId = user.Id,
            Username = user.Username,
            Role = user.Role.ToString(),
            EmployeeId = user.EmployeeId,
            ExpiresAt = expiresAt
        };
    }
}
