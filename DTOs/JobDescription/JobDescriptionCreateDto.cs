using System.ComponentModel.DataAnnotations;
namespace HRIS.Api.DTOs.JobDescription;
public class JobDescriptionCreateDto
{
    public int? OrganizationId { get; set; }
    public int? JobLevelId { get; set; }
    public int? GradeId { get; set; }
    public int? JobHolderEmployeeId { get; set; }
    public int? ImmediateManagerEmployeeId { get; set; }
    [Required,MaxLength(200)] public string JobTitle { get; set; } = "";
    [MaxLength(5000)] public string Purpose { get; set; } = "";
    [MaxLength(5000)] public string ReportingRelationship { get; set; } = "";
    [MaxLength(5000)] public string Dimensions { get; set; } = "";
    [MaxLength(5000)] public string KriCico { get; set; } = "";
    [MaxLength(5000)] public string KriCompliance { get; set; } = "";
    [MaxLength(10000)] public string KriAreas { get; set; } = "";
    [MaxLength(5000)] public string Stakeholders { get; set; } = "";
    [MaxLength(5000)] public string Challenges { get; set; } = "";
    [MaxLength(5000)] public string Qualifications { get; set; } = "";
    [MaxLength(5000)] public string Experience { get; set; } = "";
    [MaxLength(5000)] public string Competencies { get; set; } = "";
}
