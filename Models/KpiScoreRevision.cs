using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.Models;

// Audit trail: setiap kali Manager override EmployeeKpiScore.Score,
// satu baris ditambahkan ke sini (nilai lama, nilai baru, siapa yang
// ubah, kapan, alasannya). EmployeeKpiScore.Score tidak pernah "diganti
// diam-diam" tanpa jejak — lihat bab 10.3 technical alignment doc.
public class KpiScoreRevision
{
    public int Id { get; set; }

    public int EmployeeKpiScoreId { get; set; }
    public EmployeeKpiScore? EmployeeKpiScore { get; set; }

    public decimal PreviousScore { get; set; }
    public decimal NewScore { get; set; }

    // Sama seperti FilledByUserId di EmployeeKpiScore: Manager yang
    // override dicatat lewat User.Id, bukan Employee.Id.
    public int RevisedByUserId { get; set; }
    public User? RevisedByUser { get; set; }

    public DateTime RevisedAt { get; set; } = DateTime.UtcNow;

    // Wajib diisi saat override — justifikasi kenapa nilai diubah.
    [Required, MaxLength(500)]
    public string Note { get; set; } = string.Empty;
}
