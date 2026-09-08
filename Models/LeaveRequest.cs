using System.ComponentModel.DataAnnotations;
using HRIS.Api.Models.Enums;

namespace HRIS.Api.Models;

public class LeaveRequest
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }

    [MaxLength(500)]
    public string? Reason { get; set; }

    public LeaveRequestStatus Status { get; set; } = LeaveRequestStatus.Pending;

    // Level approval yang sedang aktif/ditunggu. Approver level lain
    // baru boleh action setelah level sebelumnya Approved.
    public int CurrentLevel { get; set; } = 1;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<LeaveApproval> Approvals { get; set; } = new List<LeaveApproval>();
}
