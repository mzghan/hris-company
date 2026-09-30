using HRIS.Api.Common;
using HRIS.Api.DTOs.Auth;
using HRIS.Api.Exceptions;
using HRIS.Api.Models;
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

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto, bool actorIsSupport)
    {
        if (await _userRepository.UsernameExistsAsync(dto.Username))
            throw new BadRequestException($"Username '{dto.Username}' sudah dipakai.");

        var roleNames = dto.Roles
            .Select(r => r.Trim())
            .Where(r => r.Length > 0)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var wantsSupport = roleNames.Contains(RoleNames.Support, StringComparer.OrdinalIgnoreCase);
        if (wantsSupport && !actorIsSupport)
            throw new ForbiddenException("Hanya akun Support yang boleh memberikan role Support.");

        if (dto.EmployeeId is null)
        {
            // Akun tanpa Employee hanya untuk Support (mis. akun awal dari seed).
            if (!wantsSupport)
                throw new BadRequestException("EmployeeId wajib diisi kecuali untuk akun Support.");
        }
        else
        {
            _ = await _employeeRepository.GetByIdAsync(dto.EmployeeId.Value)
                ?? throw new BadRequestException($"EmployeeId {dto.EmployeeId} tidak ditemukan.");

            if (await _userRepository.EmployeeHasAccountAsync(dto.EmployeeId.Value))
                throw new BadRequestException("Employee ini sudah punya akun login.");

            // Employee = role dasar semua orang.
            if (!roleNames.Contains(RoleNames.Employee, StringComparer.OrdinalIgnoreCase))
                roleNames.Add(RoleNames.Employee);
        }

        var roles = await _userRepository.GetRolesByNamesAsync(roleNames);
        if (roles.Count != roleNames.Count)
        {
            var unknown = roleNames.Where(n => !roles.Any(r => r.RoleName.Equals(n, StringComparison.OrdinalIgnoreCase)));
            throw new BadRequestException($"Role tidak dikenal: {string.Join(", ", unknown)}.");
        }

        var user = new User
        {
            Username = dto.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            EmployeeId = dto.EmployeeId
        };
        foreach (var role in roles)
            user.UserRoles.Add(new UserRole { Role = role });

        await _userRepository.AddAsync(user);
        _logger.LogInformation("User {Username} terdaftar dengan role {Roles}", user.Username, string.Join(",", roleNames));

        return await BuildResponseAsync(user);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _userRepository.GetByUsernameAsync(dto.Username)
            ?? throw new BadRequestException("Username atau password salah.");

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            throw new BadRequestException("Username atau password salah.");

        if (!user.IsActive)
            throw new BadRequestException("Akun ini sudah dinonaktifkan.");

        await _userRepository.UpdateLastLoginAsync(user);
        return await BuildResponseAsync(user);
    }

    private async Task<AuthResponseDto> BuildResponseAsync(User user)
    {
        var roles = user.UserRoles.Select(ur => ur.Role!.RoleName).ToList();
        var isManager = user.EmployeeId is not null
            && await _employeeRepository.HasActiveSubordinatesAsync(user.EmployeeId.Value);

        var (token, expiresAt) = _jwtService.GenerateToken(user, roles, isManager);
        return new AuthResponseDto
        {
            Token = token,
            UserId = user.Id,
            Username = user.Username,
            Roles = roles,
            IsManager = isManager,
            EmployeeId = user.EmployeeId,
            ExpiresAt = expiresAt
        };
    }
}
