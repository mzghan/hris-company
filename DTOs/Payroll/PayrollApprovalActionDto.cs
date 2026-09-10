using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.DTOs.Payroll;

public class PayrollApprovalActionDto
{
    [MaxLength(500)]
    public string? Note { get; set; }
}
