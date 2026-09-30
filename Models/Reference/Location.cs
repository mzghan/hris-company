using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.Models;

// REF_Location
public class Location
{
    public int Id { get; set; }

    [Required, MaxLength(150)]
    public string LocationName { get; set; } = string.Empty;
}
