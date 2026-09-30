using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.Models;

// REF_Address_Type
public class AddressType
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string AddressTypeName { get; set; } = string.Empty;
}
