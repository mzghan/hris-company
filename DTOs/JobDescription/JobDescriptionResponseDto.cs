namespace HRIS.Api.DTOs.JobDescription;
public class JobDescriptionResponseDto
{
    public int Id { get; set; }
    public string Code { get; set; } = "";
    public string JobTitle { get; set; } = "";
    public string OrganizationName { get; set; } = "";
    public string JobLevelName { get; set; } = "";
    public string GradeName { get; set; } = "";
    public string Status { get; set; } = "";
    public int ApprovalFlag { get; set; }
    public string Purpose { get; set; } = "";
    public string ReportingRelationship { get; set; } = "";
    public string Dimensions { get; set; } = "";
    public string KriCico { get; set; } = "";
    public string KriCompliance { get; set; } = "";
    public string KriAreas { get; set; } = "";
    public string Stakeholders { get; set; } = "";
    public string Challenges { get; set; } = "";
    public string Qualifications { get; set; } = "";
    public string Experience { get; set; } = "";
    public string Competencies { get; set; } = "";
    public string? JobHolderSignatureText { get; set; }
    public string? ManagerSignatureText { get; set; }
}
