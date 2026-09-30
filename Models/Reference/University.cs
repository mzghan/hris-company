using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.Models;

// REF_University
public class University
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string UniversityName { get; set; } = string.Empty;
}
