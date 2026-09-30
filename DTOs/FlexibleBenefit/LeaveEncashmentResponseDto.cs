namespace HRIS.Api.DTOs.FlexibleBenefit;
public class LeaveEncashmentResponseDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public int PeriodId { get; set; }
    public string PeriodName { get; set; } = string.Empty;
    public int LeaveTypeId { get; set; }
    public string LeaveTypeName { get; set; } = string.Empty;
    public int Days { get; set; }
    public string Status { get; set; } = string.Empty;
    public int? ApprovalId { get; set; }
}
