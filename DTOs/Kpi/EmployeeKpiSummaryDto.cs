namespace HRIS.Api.DTOs.Kpi;

// Dipakai untuk daftar review Manager dan untuk lihat detail satu employee:
// gabungan semua EmployeeKpiScore employee tsb pada satu periode + hasil
// Σ (Score × Weight) / 100 yang dihitung on-the-fly (tidak disimpan di DB).
public class EmployeeKpiSummaryDto
{
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public int KpiPeriodId { get; set; }
    public decimal FinalScore { get; set; }
    public List<EmployeeKpiScoreResponseDto> Scores { get; set; } = new();
}
