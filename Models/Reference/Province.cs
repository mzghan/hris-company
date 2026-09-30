using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.Models;

// REF_Province
public class Province
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string ProvinceName { get; set; } = string.Empty;

    public int CountryId { get; set; }
    public Country? Country { get; set; }
}
