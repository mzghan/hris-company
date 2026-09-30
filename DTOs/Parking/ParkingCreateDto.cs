using System.ComponentModel.DataAnnotations;
namespace HRIS.Api.DTOs.Parking;
public class ParkingCreateDto { [Range(1,int.MaxValue)] public int VehicleTypeId { get; set; } [Required, MaxLength(30)] public string PlateNumber { get; set; } = string.Empty; }
