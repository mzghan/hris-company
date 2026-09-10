namespace HRIS.Api.DTOs.Kpi;

public class KpiCriteriaResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Weight { get; set; }
}
