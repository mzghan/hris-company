using System.ComponentModel.DataAnnotations;
using HRIS.Api.Models.Enums;

namespace HRIS.Api.Models;

public class User
{
    public int Id { get; set; }

    [Required, MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public UserRole Role { get; set; }

    // Nullable: akun admin awal (seed) tidak wajib terhubung ke data Employee.
    public int? EmployeeId { get; set; }
    public Employee? Employee { get; set; }
}
