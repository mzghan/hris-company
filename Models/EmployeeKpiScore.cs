namespace HRIS.Api.Models;

// Nilai KPI satu employee, untuk satu kriteria, pada satu periode.
// Score di sini adalah nilai yang BERLAKU SAAT INI — kalau di-override
// Manager, kolom ini yang berubah (nilai lama tersimpan di
// KpiScoreRevision, bukan di kolom terpisah di sini).
//
// Skala Score belum difinalkan (lihat bab 10.4 technical alignment doc:
// 1-5, 1-100, atau huruf A/B/C/D). Untuk MVP dipakai skala numerik 0-100
// supaya gampang dikalikan langsung dengan KpiCriteria.Weight (juga 0-100)
// saat hitung final score. Tinggal diganti kalau skalanya sudah diputuskan.
public class EmployeeKpiScore
{
    public int Id { get; set; }

    public int KpiPeriodId { get; set; }
    public KpiPeriod? KpiPeriod { get; set; }

    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    public int CriteriaId { get; set; }
    public KpiCriteria? Criteria { get; set; }

    public decimal Score { get; set; }

    // User (bukan Employee) yang mengisi nilai ini — biasanya Admin,
    // dan Admin tidak selalu terhubung ke data Employee (lihat User.EmployeeId
    // yang nullable untuk akun admin awal), jadi dicatat lewat User.Id.
    public int FilledByUserId { get; set; }
    public User? FilledByUser { get; set; }

    public DateTime FilledAt { get; set; } = DateTime.UtcNow;

    public ICollection<KpiScoreRevision> Revisions { get; set; } = new List<KpiScoreRevision>();
}
