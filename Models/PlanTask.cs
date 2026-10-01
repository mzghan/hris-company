using System.ComponentModel.DataAnnotations;
namespace HRIS.Api.Models;
public class PlanTask : IAuditable
{
    public int Id { get; set; }
    public int PlanId { get; set; }
    public PerformancePlan? Plan { get; set; }
    [Required, MaxLength(250)] public string Title { get; set; } = string.Empty;
    public int AssignedByUserId { get; set; }
    public User? AssignedByUser { get; set; }
    public DateOnly? DueDate { get; set; }
    [Required, MaxLength(30)] public string Status { get; set; } = "Open";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
}