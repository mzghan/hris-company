using System.ComponentModel.DataAnnotations;
namespace HRIS.Api.DTOs.Laptop;
public class LaptopCreateDto { [Required, MaxLength(200)] public string Model { get; set; } = string.Empty; public long? Price { get; set; } }
