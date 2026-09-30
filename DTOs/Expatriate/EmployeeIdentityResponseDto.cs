namespace HRIS.Api.DTOs.Expatriate;

public class EmployeeIdentityResponseDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public int IdentityTypeId { get; set; }
    public string IdentityTypeName { get; set; } = string.Empty;
    public string IdentityNumber { get; set; } = string.Empty;
    public DateOnly? ValidUntil { get; set; }
    public bool IsPrimary { get; set; }
}
