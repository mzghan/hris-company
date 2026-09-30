using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.DTOs.Document;

public class DocumentUpdateDto
{
    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    public int CategoryId { get; set; }
}
