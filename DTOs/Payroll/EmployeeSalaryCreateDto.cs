using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.DTOs.Payroll;

public class EmployeeSalaryCreateDto
{
    [Required]
    public int EmployeeId { get; set; }

    [Range(0, double.MaxValue)]
    public decimal BaseSalary { get; set; }

    [Range(0, double.MaxValue)]
    public decimal AllowanceTotal { get; set; }

    [Required]
    public DateOnly EffectiveDate { get; set; }
}
