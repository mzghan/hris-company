using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.DTOs.Learning;

public class LearningMaterialCreateDto
{
    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    [Required]
    public int DocumentId { get; set; }
}
