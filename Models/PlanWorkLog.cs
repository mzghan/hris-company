using System.ComponentModel.DataAnnotations;
namespace HRIS.Api.Models;
public class PlanWorkLog : IAuditable
{
    public int Id { get; set; }
    public int PlanId { get; set; }
    public PerformancePlan? Plan { get; set; }
    [Required, MaxLength(2000)] public string Description { get; set; } = string.Empty;
    public DateTime LoggedAt { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
}