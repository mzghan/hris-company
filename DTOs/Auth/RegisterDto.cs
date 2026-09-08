using System.ComponentModel.DataAnnotations;
using HRIS.Api.Models.Enums;

namespace HRIS.Api.DTOs.Auth;

public class RegisterDto
{
    [Required, MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required, MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public UserRole Role { get; set; }

    // Wajib diisi kalau Role = Employee/Manager, supaya akun terhubung ke data Employee.
    public int? EmployeeId { get; set; }
}
