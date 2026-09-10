namespace HRIS.Api.DTOs.Payroll;

public class PayrollItemResponseDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public decimal BaseSalary { get; set; }
    public decimal TotalAllowance { get; set; }
    public decimal TotalDeduction { get; set; }
    public decimal GrossPay { get; set; }
    public decimal NetPay { get; set; }
}
