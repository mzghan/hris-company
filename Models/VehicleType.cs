using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.Models;

// REF_Vehicle_Type.
public class VehicleType
{
    public int Id { get; set; }
    [Required, MaxLength(100)] public string Name { get; set; } = string.Empty;
}
