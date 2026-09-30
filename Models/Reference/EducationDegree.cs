using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.Models;

// REF_Education_Degree
public class EducationDegree
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string DegreeName { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? DegreeDescription { get; set; }
}
