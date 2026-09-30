using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.Models;

// MST_Employee_WorkExperience
public class EmployeeWorkExperience : IAuditable
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    public int? IndustryId { get; set; }
    public Industry? Industry { get; set; }

    [Required, MaxLength(150)]
    public string CompanyName { get; set; } = string.Empty;

    [MaxLength(150)]
    public string? JobTitle { get; set; }

    [MaxLength(150)]
    public string? Location { get; set; }

    public int? JobLevelId { get; set; }
    public JobLevel? JobLevel { get; set; }

    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
}
