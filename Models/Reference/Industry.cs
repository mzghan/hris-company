using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.Models;

// REF_Industry
public class Industry
{
    public int Id { get; set; }

    [Required, MaxLength(150)]
    public string IndustryName { get; set; } = string.Empty;
}
