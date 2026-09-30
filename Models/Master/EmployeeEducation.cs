using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.Models;

// MST_Employee_Education
public class EmployeeEducation : IAuditable
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    public int DegreeId { get; set; }
    public EducationDegree? Degree { get; set; }

    public int? TitleId { get; set; }
    public EducationTitle? Title { get; set; }

    public int? UniversityId { get; set; }
    public University? University { get; set; }

    public int? StartYear { get; set; }
    public int? EndYear { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
}
