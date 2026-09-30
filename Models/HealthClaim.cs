using System.ComponentModel.DataAnnotations;
namespace HRIS.Api.Models;
public class HealthClaim : IAuditable
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }
    public int PeriodId { get; set; }
    public FlexPeriod? Period { get; set; }
    public long Amount { get; set; }
    [MaxLength(500)] public string? Description { get; set; }
    [Required, MaxLength(30)] public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
}
