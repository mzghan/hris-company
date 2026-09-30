using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.DTOs.Expatriate;

public class EmployeeIdentityCreateDto
{
    [Required]
    public int IdentityTypeId { get; set; }

    [Required, MaxLength(50)]
    public string IdentityNumber { get; set; } = string.Empty;

    public DateOnly? ValidUntil { get; set; }
    public bool IsPrimary { get; set; }
}
