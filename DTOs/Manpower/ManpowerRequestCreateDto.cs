using System.ComponentModel.DataAnnotations;
namespace HRIS.Api.DTOs.Manpower;
public class ManpowerRequestCreateDto
{
    [Required] public int OrganizationId { get; set; }
    [Required] public int JobTitleId { get; set; }
    [Required] public int JobLevelId { get; set; }
    [Required] public int EmploymentTypeId { get; set; }
    [Required] public string RequestType { get; set; } = "NewHeadcount";
    public int? ReplacementForEmployeeId { get; set; }
    public int? ReportToEmployeeId { get; set; }
    [Required] public string WorkStatus { get; set; } = "FTE";
    public int? JobDescriptionId { get; set; }
    [Range(1,1000)] public int Headcount { get; set; }
    [Required, MaxLength(1000)] public string Reason { get; set; } = string.Empty;
    [Required] public DateOnly TargetDate { get; set; }
}
