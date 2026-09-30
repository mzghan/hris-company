using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.Models;

// REF_Religion
public class Religion
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string ReligionName { get; set; } = string.Empty;
}
