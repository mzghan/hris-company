using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.DTOs.Assistance;

public class AssistanceStatusUpdateDto
{
    [Required, MaxLength(30)]
    public string Status { get; set; } = string.Empty;
}
