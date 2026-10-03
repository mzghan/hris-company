using System.ComponentModel.DataAnnotations;
namespace HRIS.Api.Models;
public class ManpowerRequest : IAuditable
{
    public int Id { get; set; }
    public int RequestedByEmployeeId { get; set; }
    [MaxLength(20)] public string RequestType { get; set; } = "NewHeadcount";
    public int? ReplacementForEmployeeId { get; set; }
    public Employee? ReplacementForEmployee { get; set; }
    public int? ReportToEmployeeId { get; set; }
    public Employee? ReportToEmployee { get; set; }
    public Employee? RequestedByEmployee { get; set; }
    public int OrganizationId { get; set; }
    public Organization? Organization { get; set; }
    public int JobTitleId { get; set; }
    public JobTitle? JobTitle { get; set; }
    public int JobLevelId { get; set; }
    public JobLevel? JobLevel { get; set; }
    public int EmploymentTypeId { get; set; }
    [MaxLength(20)] public string WorkStatus { get; set; } = "FTE";
    public int? JobDescriptionId { get; set; }
    public JobDescription? JobDescription { get; set; }
    [MaxLength(500)] public string? JobDescriptionPath { get; set; }
    [MaxLength(500)] public string? BusinessPlanPath { get; set; }
    [MaxLength(500)] public string? ManualMrfPath { get; set; }
    [MaxLength(50)] public string ApprovalPhase { get; set; } = "Waiting Approval";
    public EmploymentType? EmploymentType { get; set; }
    public int Headcount { get; set; }
    [Required, MaxLength(1000)] public string Reason { get; set; } = string.Empty;
    public DateOnly TargetDate { get; set; }
    [Required, MaxLength(30)] public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
    public ICollection<ManpowerRequestRemark> Remarks { get; set; } = new List<ManpowerRequestRemark>();
    public ICollection<ManpowerRequestFiling> Filings { get; set; } = new List<ManpowerRequestFiling>();
    public ICollection<ManpowerVacancy> Vacancies { get; set; } = new List<ManpowerVacancy>();
}
