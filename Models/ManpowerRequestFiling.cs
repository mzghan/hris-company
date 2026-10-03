using System.ComponentModel.DataAnnotations;
namespace HRIS.Api.Models;
public class ManpowerRequestFiling : IAuditable
{
    public int Id { get; set; }
    public int ManpowerRequestId { get; set; }
    public ManpowerRequest? ManpowerRequest { get; set; }
    [Required, MaxLength(100)] public string FileType { get; set; } = string.Empty;
    [Required, MaxLength(255)] public string FileName { get; set; } = string.Empty;
    [Required, MaxLength(500)] public string StoredPath { get; set; } = string.Empty;
    [MaxLength(150)] public string? ContentType { get; set; }
    public long SizeBytes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
}
