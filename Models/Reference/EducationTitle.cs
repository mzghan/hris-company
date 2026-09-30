using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.Models;

// REF_Education_Title
public class EducationTitle
{
    public int Id { get; set; }

    [Required, MaxLength(150)]
    public string TitleName { get; set; } = string.Empty;

    [MaxLength(150)]
    public string? MajorName { get; set; }
}
