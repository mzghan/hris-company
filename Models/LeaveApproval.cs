using System.ComponentModel.DataAnnotations;
using HRIS.Api.Models.Enums;

namespace HRIS.Api.Models;

public class LeaveApproval
{
    public int Id { get; set; }

    public int LeaveRequestId { get; set; }
    public LeaveRequest? LeaveRequest { get; set; }

    // Employee yang bertindak sebagai approver di level ini.
    public int ApproverId { get; set; }
    public Employee? Approver { get; set; }

    public int Level { get; set; }
    public ApprovalStatus Status { get; set; } = ApprovalStatus.Pending;

    [MaxLength(500)]
    public string? Note { get; set; }

    public DateTime? ActedAt { get; set; }
}
