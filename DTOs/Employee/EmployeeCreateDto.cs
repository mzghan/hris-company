using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.DTOs.Employee;

public class EmployeeCreateDto
{
    [Required, MaxLength(150)]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Position { get; set; }

    [Required]
    public DateTime HireDate { get; set; }

    public int? DepartmentId { get; set; }
    public int? ManagerId { get; set; }
}
