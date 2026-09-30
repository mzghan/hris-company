using System.ComponentModel.DataAnnotations;
using HRIS.Api.Models.Enums;

namespace HRIS.Api.Models;

// TRX_Approval_Step. Snapshot approver yang di-resolve saat submit: kalau atasan
// berubah kemudian, riwayat approval tetap menunjuk orang yang memang ditugaskan saat itu.
public class ApprovalStep : IAuditable
{
    public int Id { get; set; }

    public int ApprovalId { get; set; }
    public ApprovalRequest? Approval { get; set; }

    public int Level { get; set; }

    // Role atau Employee (ManagerChain sudah jadi Employee).
    public ApproverType ApproverType { get; set; }

    public int? ApproverRoleId { get; set; }
    public Role? ApproverRole { get; set; }

    public int? ApproverEmployeeId { get; set; }
    public Employee? ApproverEmployee { get; set; }

    public ApprovalStepStatus Status { get; set; } = ApprovalStepStatus.Pending;

    // User yang benar-benar menekan Approve/Reject. Untuk step bertipe Role, siapa pun
    // pemegang role boleh; untuk Employee bisa berbeda dari approver kalau Support yang bertindak.
    public int? ActedByUserId { get; set; }
    public User? ActedByUser { get; set; }

    // True kalau Support bertindak padahal bukan approver yang ditugaskan (tercatat juga di audit log).
    public bool IsSupportOverride { get; set; }

    public DateTime? ActedAt { get; set; }

    [MaxLength(500)]
    public string? Note { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
}
