using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.Models;

// Master data kriteria penilaian KPI (mis. "Kedisiplinan" bobot 30%).
// Dikelola Admin. Total Weight semua kriteria idealnya 100%, tapi
// tidak dipaksa lewat constraint DB — lihat catatan di KpiService
// (bab 10.1 technical alignment doc, kata "idealnya" bukan "harus").
public class KpiCriteria
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    // Disimpan sebagai persen (0-100), bukan pecahan (0-1), supaya
    // konsisten dengan cara ditulis di technical alignment doc ("bobot 30%").
    [Range(0, 100)]
    public decimal Weight { get; set; }
}
