namespace HRIS.Api.DTOs.Payroll;

public class EmployeeSalaryResponseDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public decimal BaseSalary { get; set; }
    public decimal AllowanceTotal { get; set; }
    public DateOnly EffectiveDate { get; set; }
    public DateTime CreatedAt { get; set; }
}
