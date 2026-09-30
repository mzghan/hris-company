namespace HRIS.Api.Models.Enums;

public enum ApprovalStepStatus
{
    Pending,
    Approved,
    Rejected,

    // Langkah yang tidak pernah dijalankan karena pengajuan sudah Rejected/Cancelled lebih dulu.
    Skipped
}
