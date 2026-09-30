using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.DTOs.Document;

public class DocumentCategoryCreateDto
{
    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    // Null = kategori paling atas.
    public int? ParentId { get; set; }
}
