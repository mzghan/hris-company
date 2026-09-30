namespace HRIS.Api.DTOs.FlexibleBenefit;
public class HealthClaimResponseDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public int PeriodId { get; set; }
    public string PeriodName { get; set; } = string.Empty;
    public long Amount { get; set; }
    public string? Description { get; set; }
    public string Status { get; set; } = string.Empty;
    public int? ApprovalId { get; set; }
}
