using System.ComponentModel.DataAnnotations;
using HRIS.Api.Models.Enums;

namespace HRIS.Api.Models;

// Satu baris = satu periode penilaian (mis. "Q1 2027").
public class KpiPeriod
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public int Year { get; set; }

    public KpiPeriodStatus Status { get; set; } = KpiPeriodStatus.Draft;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<EmployeeKpiScore> Scores { get; set; } = new List<EmployeeKpiScore>();
}
