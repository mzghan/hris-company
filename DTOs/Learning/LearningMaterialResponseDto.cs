namespace HRIS.Api.DTOs.Learning;

public class LearningMaterialResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DocumentId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string? ContentType { get; set; }
    public long SizeBytes { get; set; }
    public DateTime CreatedAt { get; set; }
}
