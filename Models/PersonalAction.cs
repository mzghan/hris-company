using System.ComponentModel.DataAnnotations;
namespace HRIS.Api.Models;
public class PersonalAction : IAuditable
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }
    [Required, MaxLength(30)] public string ActionType { get; set; } = string.Empty;
    public DateOnly EffectiveDate { get; set; }
    [Required, MaxLength(1000)] public string Reason { get; set; } = string.Empty;
    [Required, MaxLength(30)] public string Status { get; set; } = "Pending";
    public int? NewOrganizationId { get; set; }
    public Organization? NewOrganization { get; set; }
    public int? NewJobTitleId { get; set; }
    public JobTitle? NewJobTitle { get; set; }
    public int? NewJobLevelId { get; set; }
    public JobLevel? NewJobLevel { get; set; }
    public int? NewGradeId { get; set; }
    public Grade? NewGrade { get; set; }
    public int? NewLocationId { get; set; }
    public Location? NewLocation { get; set; }
    public int? NewManagerId { get; set; }
    public Employee? NewManager { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
}