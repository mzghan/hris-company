using System.ComponentModel.DataAnnotations;
namespace HRIS.Api.DTOs.FlexibleBenefit;
public class HealthClaimCreateDto
{
    [Required] public int PeriodId { get; set; }
    [Range(1, long.MaxValue)] public long Amount { get; set; }
    [MaxLength(500)] public string? Description { get; set; }
}
