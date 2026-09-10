using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.DTOs.Kpi;

public class KpiCriteriaCreateDto
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Range(0, 100)]
    public decimal Weight { get; set; }
}
