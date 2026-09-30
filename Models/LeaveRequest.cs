using System.ComponentModel.DataAnnotations;
using HRIS.Api.Models.Enums;

namespace HRIS.Api.Models;

// TRX_Leave_Request. Persetujuan tidak lagi disimpan di sini: approver dan langkahnya ada
// di approval engine (TRX_Approval_Request dengan request_type "Leave" dan request_ref_id = Id).
// Status di sini hanya cerminan hasil akhir, diisi oleh LeaveApprovalHandler.
public class LeaveRequest : IAuditable
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    public int LeaveTypeId { get; set; }
    public LeaveType? LeaveType { get; set; }

    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }

    // Jumlah hari kerja (Senin-Jumat). Dasar aturan "cuti panjang" di alur approval.
    // Libur nasional baru dikurangi setelah REF_Public_Holiday ada (Batch D).
    public int Days { get; set; }

    [MaxLength(500)]
    public string? Reason { get; set; }

    public LeaveRequestStatus Status { get; set; } = LeaveRequestStatus.Pending;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
}
