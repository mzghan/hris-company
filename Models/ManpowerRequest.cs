using System.ComponentModel.DataAnnotations;
namespace HRIS.Api.Models;
public class ManpowerRequest : IAuditable
{
    public int Id { get; set; }
    public int RequestedByEmployeeId { get; set; }
    public Employee? RequestedByEmployee { get; set; }
    public int OrganizationId { get; set; }
    public Organization? Organization { get; set; }
    public int JobTitleId { get; set; }
    public JobTitle? JobTitle { get; set; }
    public int JobLevelId { get; set; }
    public JobLevel? JobLevel { get; set; }
    public int EmploymentTypeId { get; set; }
    public EmploymentType? EmploymentType { get; set; }
    public int Headcount { get; set; }
    [Required, MaxLength(1000)] public string Reason { get; set; } = string.Empty;
    public DateOnly TargetDate { get; set; }
    [Required, MaxLength(30)] public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
}
