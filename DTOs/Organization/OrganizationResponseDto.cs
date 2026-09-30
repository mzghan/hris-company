namespace HRIS.Api.DTOs.Organization;

public class OrganizationResponseDto
{
    public int Id { get; set; }
    public string OrganizationName { get; set; } = string.Empty;
    public int? ParentId { get; set; }
    public string? ParentName { get; set; }
    public int OrganizationLevel { get; set; }
    public string Path { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int EmployeeCount { get; set; }
}
