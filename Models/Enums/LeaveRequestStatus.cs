namespace HRIS.Api.Models.Enums;

// Mencerminkan hasil akhir di approval engine (TRX_Approval_Request), diisi oleh LeaveApprovalHandler.
public enum LeaveRequestStatus
{
    Pending,
    Approved,
    Rejected,
    Cancelled
}
