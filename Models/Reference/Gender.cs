using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.Models;

// REF_Gender
public class Gender
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string GenderName { get; set; } = string.Empty;
}
