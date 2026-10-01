namespace HRIS.Api.Common;

// Nilai TRX_Approval_Request.request_type. Sengaja string (bukan enum DB) supaya
// modul baru cukup menambah konstanta + seed REF_Approval_Flow_Step tanpa migration.
public static class ApprovalRequestTypes
{
    public const string Leave = "Leave";
    public const string FamilyChange = "FamilyChange";
    public const string LeaveEncashment = "LeaveEncashment";
    public const string HealthClaim = "HealthClaim";
    public const string Letter = "Letter";
    public const string Parking = "Parking";
    public const string Laptop = "Laptop";
    public const string PersonalAction = "PersonalAction";
    public const string Manpower = "Manpower";

    public static string Label(string type) => type switch
    {
        Leave => "Cuti",
        FamilyChange => "Perubahan Data Keluarga",
        LeaveEncashment => "Penjualan Cuti",
        HealthClaim => "Klaim Kesehatan",
        Letter => "Surat HR",
        Parking => "Registrasi Parkir",
        Laptop => "Kepemilikan Laptop",
        PersonalAction => "Personal Action",
        Manpower => "Permintaan Tenaga Kerja",
        _ => type
    };

    // Halaman tujuan requester saat notifikasi hasil approval diklik.
    public static string RequesterLink(string type) => type switch
    {
        Leave => "/Leave/Index",
        LeaveEncashment => "/FlexibleBenefits/Index",
        HealthClaim => "/FlexibleBenefits/Index",
        PersonalAction => "/PersonalActions/Index",
        Manpower => "/Manpower/Index",
        _ => "/Approvals/Index"
    };
}
