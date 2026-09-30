using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.DTOs.Assistance;

public class AssistanceRequestCreateDto
{
    [Required, MaxLength(100)]
    public string Category { get; set; } = string.Empty;

    [Required, MaxLength(4000)]
    public string Description { get; set; } = string.Empty;

    public bool IsAnonymous { get; set; }
}
