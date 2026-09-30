using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.Models;

// TRX_Family_Change_Request. MST_Employee_Family hanya berubah setelah approval selesai.
public class FamilyChangeRequest : IAuditable
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }
    public int? FamilyId { get; set; }
    public EmployeeFamily? Family { get; set; }
    [Required, MaxLength(20)] public string Action { get; set; } = "Add";
    [Required, MaxLength(150)] public string FamilyName { get; set; } = string.Empty;
    public int RelationshipId { get; set; }
    public Relationship? Relationship { get; set; }
    public int? GenderId { get; set; }
    public Gender? Gender { get; set; }
    public DateOnly? BirthDate { get; set; }
    public bool IsDependent { get; set; }
    public DateOnly? WeddingDate { get; set; }
    [Required, MaxLength(20)] public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
}
