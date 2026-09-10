namespace HRIS.Api.DTOs.Kpi;

public class KpiScoreRevisionResponseDto
{
    public int Id { get; set; }
    public decimal PreviousScore { get; set; }
    public decimal NewScore { get; set; }
    public int RevisedByUserId { get; set; }
    public string RevisedByUsername { get; set; } = string.Empty;
    public DateTime RevisedAt { get; set; }
    public string Note { get; set; } = string.Empty;
}
