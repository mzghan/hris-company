using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.DTOs.Auth;

public class RegisterDto
{
    [Required, MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required, MinLength(6)]
    public string Password { get; set; } = string.Empty;

    // Role tambahan: "HR" atau "Support". Role "Employee" otomatis ditambahkan
    // kalau EmployeeId diisi. Role "Support" hanya boleh diberikan oleh akun Support.
    public List<string> Roles { get; set; } = new();

    // Wajib diisi kecuali untuk akun Support murni (mis. akun awal dari seed).
    public int? EmployeeId { get; set; }
}
