namespace HRIS.Api.DTOs.Expatriate;

public class ExpatriateEmployeeResponseDto
{
    public int EmployeeId { get; set; }
    public string EmployeeNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Nationality { get; set; } = string.Empty;
    public string? WorkEmail { get; set; }
    public DateOnly JoinDate { get; set; }
    public List<EmployeeIdentityResponseDto> Identities { get; set; } = new();
}
