using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.DTOs.Kpi;

public class KpiPeriodCreateDto
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Range(2000, 2100)]
    public int Year { get; set; }
}
