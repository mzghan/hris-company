using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.Models;

// MST_Employee_Identity. Juga dipakai untuk Passport/KITAS/KITAP (modul 19) dan nomor BPJS. Reminder dari ValidUntil.
public class EmployeeIdentity : IAuditable
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    public int IdentityTypeId { get; set; }
    public IdentityType? IdentityType { get; set; }

    [Required, MaxLength(50)]
    public string IdentityNumber { get; set; } = string.Empty;

    public DateOnly? ValidUntil { get; set; }
    public bool IsPrimary { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
}
