using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.DTOs.Leave;

public class LeaveApprovalActionDto
{
    [MaxLength(500)]
    public string? Note { get; set; }
}
