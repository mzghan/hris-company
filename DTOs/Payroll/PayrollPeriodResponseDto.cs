namespace HRIS.Api.DTOs.Payroll;

public class PayrollPeriodResponseDto
{
    public int Id { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public string Status { get; set; } = string.Empty;
    public int CurrentLevel { get; set; }
    public DateTime CreatedAt { get; set; }
    public decimal TotalNetPay { get; set; }
    public List<PayrollItemResponseDto> Items { get; set; } = new();
    public List<PayrollApprovalResponseDto> Approvals { get; set; } = new();
}
