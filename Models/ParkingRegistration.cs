using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.Models;

// TRX_Parking_Registration. Approval dilakukan melalui approval engine.
public class ParkingRegistration : IAuditable
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }
    public int VehicleTypeId { get; set; }
    public VehicleType? VehicleType { get; set; }
    [Required, MaxLength(30)] public string PlateNumber { get; set; } = string.Empty;
    [Required, MaxLength(20)] public string Status { get; set; } = "Pending";
    public DateTime? CardReadyAt { get; set; }
    public DateTime? CardCollectedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
}
