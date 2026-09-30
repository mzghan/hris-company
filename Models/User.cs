using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.Models;

// SYS_User. Satu akun = satu Employee (unique). Employee boleh null hanya
// untuk akun Support awal (seed), yang belum tentu punya data karyawan.
public class User
{
    public int Id { get; set; }

    [Required, MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginAt { get; set; }

    public int? EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
