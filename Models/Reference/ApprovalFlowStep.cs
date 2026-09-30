using System.ComponentModel.DataAnnotations;
using HRIS.Api.Models.Enums;

namespace HRIS.Api.Models;

// REF_Approval_Flow_Step. Alur approval tiap jenis pengajuan disimpan sebagai
// konfigurasi (bukan hardcode). Langkah di-resolve menjadi approver konkret saat submit.
public class ApprovalFlowStep
{
    public int Id { get; set; }

    [Required, MaxLength(50)]
    public string RequestType { get; set; } = string.Empty;

    public int Level { get; set; }

    public ApproverType ApproverType { get; set; }

    // Untuk ManagerChain: naik berapa tingkat (1 = atasan langsung, 2 = Head, dst).
    // Kalau rantai atasan habis sebelum depth tercapai, langkah ini dilewati.
    public int? ChainDepth { get; set; }

    // Untuk tipe Role.
    public int? RoleId { get; set; }
    public Role? Role { get; set; }

    // Untuk tipe Employee (approver tetap).
    public int? ApproverEmployeeId { get; set; }
    public Employee? ApproverEmployee { get; set; }

    // Langkah hanya berlaku kalau jumlah hari pengajuan >= nilai ini (mis. cuti panjang).
    public int? MinRequestedDays { get; set; }

    public bool IsActive { get; set; } = true;
}
