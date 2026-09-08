using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.Models;

public class Employee
{
    public int Id { get; set; }

    [Required, MaxLength(150)]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Position { get; set; }

    public DateTime HireDate { get; set; }

    public bool IsActive { get; set; } = true;

    // Nullable: employee boleh belum punya department saat baru dibuat
    public int? DepartmentId { get; set; }
    public Department? Department { get; set; }

    // Self-referencing: atasan langsung. Null berarti dia paling atas (mis. Direktur).
    public int? ManagerId { get; set; }
    public Employee? Manager { get; set; }
    public ICollection<Employee> Subordinates { get; set; } = new List<Employee>();
}
