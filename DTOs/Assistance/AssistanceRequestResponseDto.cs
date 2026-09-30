namespace HRIS.Api.DTOs.Assistance;

public class AssistanceRequestResponseDto
{
    public int Id { get; set; }
    public int? EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsAnonymous { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? HandledByName { get; set; }
    public DateTime CreatedAt { get; set; }
}
