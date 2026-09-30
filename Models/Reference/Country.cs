using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.Models;

// REF_Country. Ekspatriat ditentukan dari Employee.NationalityCountryId bukan Indonesia (kode "ID").
public class Country
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string CountryName { get; set; } = string.Empty;

    [Required, MaxLength(5)]
    public string CountryCode { get; set; } = string.Empty;
}
