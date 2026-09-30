namespace HRIS.Api.DTOs.Manpower;
public class ManpowerRequestResponseDto
{
    public int Id { get; set; }
    public int RequestedByEmployeeId { get; set; }
    public string RequestedByName { get; set; } = string.Empty;
    public int OrganizationId { get; set; }
    public string OrganizationName { get; set; } = string.Empty;
    public int JobTitleId { get; set; }
    public string JobTitleName { get; set; } = string.Empty;
    public int JobLevelId { get; set; }
    public string JobLevelName { get; set; } = string.Empty;
    public int EmploymentTypeId { get; set; }
    public string EmploymentTypeName { get; set; } = string.Empty;
    public int Headcount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateOnly TargetDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public int? ApprovalId { get; set; }
}
