namespace HRIS.Api.DTOs.Kpi;

public class EmployeeKpiScoreResponseDto
{
    public int Id { get; set; }
    public int KpiPeriodId { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public int CriteriaId { get; set; }
    public string CriteriaName { get; set; } = string.Empty;
    public decimal CriteriaWeight { get; set; }
    public decimal Score { get; set; }
    public int FilledByUserId { get; set; }
    public string FilledByUsername { get; set; } = string.Empty;
    public DateTime FilledAt { get; set; }

    // Kosong kalau nilai ini belum pernah di-override Manager.
    public List<KpiScoreRevisionResponseDto> Revisions { get; set; } = new();
}
