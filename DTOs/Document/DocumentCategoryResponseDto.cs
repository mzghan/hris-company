namespace HRIS.Api.DTOs.Document;

public class DocumentCategoryResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? ParentId { get; set; }

    // Nama lengkap berjenjang, mis. "Forms > Medical".
    public string FullName { get; set; } = string.Empty;
    public int DocumentCount { get; set; }
}
