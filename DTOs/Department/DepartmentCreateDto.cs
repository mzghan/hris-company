using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.DTOs.Department;

public class DepartmentCreateDto
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;
}
