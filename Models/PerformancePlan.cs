using System.ComponentModel.DataAnnotations;
namespace HRIS.Api.Models;
public class PerformancePlan : IAuditable
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    [Required, MaxLength(30)] public string Status { get; set; } = "Draft";
    public int CreatedByUserId { get; set; }
    public User? CreatedByUser { get; set; }
    public ICollection<PlanTask> Tasks { get; set; } = new List<PlanTask>();
    public ICollection<PlanWorkLog> WorkLogs { get; set; } = new List<PlanWorkLog>();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
}