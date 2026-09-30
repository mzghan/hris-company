using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.Models;

// MST_Employee_Family. Hanya berisi data yang sudah disetujui HR (perubahan lewat TRX_Family_Change_Request di batch berikutnya).
public class EmployeeFamily : IAuditable
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    public int RelationshipId { get; set; }
    public Relationship? Relationship { get; set; }

    public int? GenderId { get; set; }
    public Gender? Gender { get; set; }

    [Required, MaxLength(150)]
    public string FamilyName { get; set; } = string.Empty;

    public DateOnly? BirthDate { get; set; }
    public bool IsDependent { get; set; }
    public bool IsSameCompany { get; set; }
    public DateOnly? WeddingDate { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
}
