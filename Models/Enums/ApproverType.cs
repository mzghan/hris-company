namespace HRIS.Api.Models.Enums;

// Di REF_Approval_Flow_Step (konfigurasi) ketiganya dipakai. Di TRX_Approval_Step
// (snapshot) ManagerChain sudah di-resolve menjadi Employee, jadi hanya Role/Employee.
public enum ApproverType
{
    // Naik N tingkat lewat Direct Manager (chain_depth). Depth 1 = atasan langsung.
    ManagerChain,
    Role,
    Employee
}
