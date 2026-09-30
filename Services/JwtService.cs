using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HRIS.Api.Models;
using Microsoft.IdentityModel.Tokens;

namespace HRIS.Api.Services;

public class JwtService : IJwtService
{
    private readonly IConfiguration _config;

    public JwtService(IConfiguration config)
    {
        _config = config;
    }

    public (string token, DateTime expiresAt) GenerateToken(User user, IEnumerable<string> roles, bool isManager)
    {
        var jwtSection = _config.GetSection("Jwt");
        var key = jwtSection["Key"]!;
        var issuer = jwtSection["Issuer"]!;
        var audience = jwtSection["Audience"]!;
        var expiresMinutes = int.Parse(jwtSection["ExpiresMinutes"] ?? "120");

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Username),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username)
        };

        // Satu claim Role per role tersimpan (Employee/HR/Support).
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        // Manager/Head/Group Head bukan role: claim ini diturunkan dari rantai
        // atasan (punya bawahan aktif di MST_Employee_Hierarchy) saat login.
        if (isManager)
        {
            claims.Add(new Claim("IsManager", "true"));
        }

        if (user.EmployeeId is not null)
        {
            // Claim custom "employeeId" dipakai di controller untuk tahu
            // employee mana yang sedang login (mis. saat check-in absen).
            claims.Add(new Claim("employeeId", user.EmployeeId.Value.ToString()));
        }

        var expiresAt = DateTime.UtcNow.AddMinutes(expiresMinutes);
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}
