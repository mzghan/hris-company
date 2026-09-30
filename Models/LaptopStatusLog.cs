using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.Models;

// TRX_Laptop_Status_Log.
public class LaptopStatusLog : IAuditable
{
    public int Id { get; set; }
    public int LaptopId { get; set; }
    public LaptopRequest? Laptop { get; set; }
    [Required, MaxLength(30)] public string Status { get; set; } = string.Empty;
    public int ChangedBy { get; set; }
    public User? ChangedByUser { get; set; }
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    [MaxLength(500)] public string? Note { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
}
