using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.DTOs.Employee;

public class EmployeeUpdateDto
{
    [Required, MaxLength(150)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Position { get; set; }

    public int? DepartmentId { get; set; }
    public int? ManagerId { get; set; }
    public bool IsActive { get; set; } = true;
}
