using System.ComponentModel.DataAnnotations;
namespace HRIS.Api.Models;
public class LeaveBalance : IAuditable
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }
    public int LeaveTypeId { get; set; }
    public LeaveType? LeaveType { get; set; }
    public int Year { get; set; }
    public int Entitlement { get; set; }
    public int Used { get; set; }
    public int Sold { get; set; }
    public int CarriedOver { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
}
