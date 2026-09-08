namespace HRIS.Api.DTOs.Employee;

public class EmployeeResponseDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Position { get; set; }
    public DateTime HireDate { get; set; }
    public bool IsActive { get; set; }
    public int? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public int? ManagerId { get; set; }
    public string? ManagerName { get; set; }
}
