using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.Models;

// REF_Contact_Type. Seed: Work Email, Personal Email, Mobile.
public class ContactType
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string ContactTypeName { get; set; } = string.Empty;
}
