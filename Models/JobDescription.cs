using System.ComponentModel.DataAnnotations;
namespace HRIS.Api.Models;
public class JobDescription : IAuditable
{
    public int Id { get; set; }
    [Required, MaxLength(100)] public string Code { get; set; } = string.Empty;
    [Required, MaxLength(200)] public string JobTitle { get; set; } = string.Empty;
    public int? OrganizationId { get; set; }
    public Organization? Organization { get; set; }
    public int? JobLevelId { get; set; }
    public JobLevel? JobLevel { get; set; }
    public int? GradeId { get; set; }
    public Grade? Grade { get; set; }
    public int? JobHolderEmployeeId { get; set; }
    public Employee? JobHolderEmployee { get; set; }
    public int? ImmediateManagerEmployeeId { get; set; }
    public Employee? ImmediateManagerEmployee { get; set; }
    public string Purpose { get; set; } = string.Empty;
    public string ReportingRelationship { get; set; } = string.Empty;
    public string Dimensions { get; set; } = string.Empty;
    public string KriCico { get; set; } = string.Empty;
    public string KriCompliance { get; set; } = string.Empty;
    public string KriAreas { get; set; } = string.Empty;
    public string Stakeholders { get; set; } = string.Empty;
    public string Challenges { get; set; } = string.Empty;
    public string Qualifications { get; set; } = string.Empty;
    public string Experience { get; set; } = string.Empty;
    public string Competencies { get; set; } = string.Empty;
    [MaxLength(30)] public string Status { get; set; } = "Draft";
    public int ApprovalFlag { get; set; } = -1;
    public string ApproversJson { get; set; } = "[]";
    public string ApproverHistoryJson { get; set; } = "[]";
    public string? JobHolderSignatureText { get; set; }
    public DateTime? DateSignJobHolder { get; set; }
    public string? ManagerSignatureText { get; set; }
    public DateTime? DateSignManager { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
    public ICollection<JobDescriptionRevision> Revisions { get; set; } = new List<JobDescriptionRevision>();
}
