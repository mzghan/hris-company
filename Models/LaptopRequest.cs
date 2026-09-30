using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.Models;

// TRX_Laptop_Request. Status mengikuti desain: Submitted -> ForwardedToIT -> Approved -> Purchased -> ResetDone.
public class LaptopRequest : IAuditable
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }
    [MaxLength(50)] public string? AssetTag { get; set; }
    [Required, MaxLength(200)] public string Model { get; set; } = string.Empty;
    public long? Price { get; set; }
    [Required, MaxLength(30)] public string Status { get; set; } = "Submitted";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
    public List<LaptopStatusLog> StatusLogs { get; set; } = new();
}
