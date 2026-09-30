namespace HRIS.Api.DTOs.Document;

public class DocumentResponseDto
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string? ContentType { get; set; }
    public long SizeBytes { get; set; }
    public int? OwnerEmployeeId { get; set; }
    public string? OwnerEmployeeName { get; set; }
    public DateTime CreatedAt { get; set; }
}
