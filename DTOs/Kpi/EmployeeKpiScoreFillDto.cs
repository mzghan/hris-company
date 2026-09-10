using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.DTOs.Kpi;

public class KpiScoreItemDto
{
    [Required]
    public int CriteriaId { get; set; }

    // Lihat catatan skala di Models/EmployeeKpiScore.cs — 0-100 untuk MVP.
    [Range(0, 100)]
    public decimal Score { get; set; }
}

// Admin isi (atau update ulang, selama period masih Draft/InReview) nilai
// satu employee untuk sejumlah kriteria sekaligus, supaya tidak perlu satu
// request per kriteria.
public class EmployeeKpiScoreFillDto
{
    [Required]
    public int EmployeeId { get; set; }

    [Required, MinLength(1)]
    public List<KpiScoreItemDto> Scores { get; set; } = new();
}
