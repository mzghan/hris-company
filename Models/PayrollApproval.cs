using System.ComponentModel.DataAnnotations;
using HRIS.Api.Models.Enums;

namespace HRIS.Api.Models;

// Struktur identik dengan LeaveApproval, tapi menempel ke PayrollPeriod
// (bukan ke tiap PayrollItem) — supaya approver cukup approve sekali
// untuk satu periode, bukan approve satu-satu per employee.
public class PayrollApproval
{
    public int Id { get; set; }

    public int PayrollPeriodId { get; set; }
    public PayrollPeriod? PayrollPeriod { get; set; }

    // Employee yang bertindak sebagai approver di level ini.
    public int ApproverId { get; set; }
    public Employee? Approver { get; set; }

    public int Level { get; set; }
    public ApprovalStatus Status { get; set; } = ApprovalStatus.Pending;

    [MaxLength(500)]
    public string? Note { get; set; }

    public DateTime? ActedAt { get; set; }
}
