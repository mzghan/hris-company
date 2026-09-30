using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.Models;

// REF_City
public class City
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string CityName { get; set; } = string.Empty;

    public int ProvinceId { get; set; }
    public Province? Province { get; set; }
}
