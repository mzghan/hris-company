using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.DTOs.Regulation;

public class RegulationCreateDto
{
    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    public int SortOrder { get; set; }
}
