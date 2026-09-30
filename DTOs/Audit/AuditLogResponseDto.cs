namespace HRIS.Api.DTOs.Audit;

public class AuditLogResponseDto
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public string? Username { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? EntityType { get; set; }
    public int? EntityId { get; set; }
    public string? Detail { get; set; }
    public DateTime CreatedAt { get; set; }
}
