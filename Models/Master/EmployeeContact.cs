using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.Models;

// MST_Employee_Contact. Email kerja = baris dengan ContactType "Work Email" (menggantikan Employee.Email lama).
public class EmployeeContact : IAuditable
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    public int ContactTypeId { get; set; }
    public ContactType? ContactType { get; set; }

    [Required, MaxLength(150)]
    public string ContactValue { get; set; } = string.Empty;

    public bool IsPrimary { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
}
