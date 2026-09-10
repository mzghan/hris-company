using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.DTOs.Payroll;

public class PayrollPeriodCreateDto
{
    [Range(1, 12)]
    public int Month { get; set; }

    [Range(2000, 2100)]
    public int Year { get; set; }
}
