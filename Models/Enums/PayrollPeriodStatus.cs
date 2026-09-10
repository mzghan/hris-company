namespace HRIS.Api.Models.Enums;

// Rejected ditambahkan di luar 4 status yang disebut di technical alignment doc
// (Draft/InApproval/Approved/Paid), supaya perilakunya simetris dengan
// LeaveRequestStatus: kalau satu level approval reject, periode berhenti
// dengan status jelas alih-alih "nyangkut" di InApproval.
public enum PayrollPeriodStatus
{
    Draft,
    InApproval,
    Approved,
    Rejected,
    Paid
}
