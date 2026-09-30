using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.Models;

// TRX_Service_Award. Satu employee dapat memiliki satu baris untuk setiap kelipatan 5 tahun.
public class ServiceAward : IAuditable
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }
    public int ServiceYears { get; set; }
    public DateOnly DueDate { get; set; }
    [Required, MaxLength(20)] public string Status { get; set; } = "Pending";
    public DateTime? GivenAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
}
