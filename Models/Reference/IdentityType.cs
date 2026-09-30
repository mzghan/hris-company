using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.Models;

// REF_Identity_Type. Seed: KTP, Passport, KITAS, KITAP, NPWP, BPJS Kesehatan, BPJS Ketenagakerjaan.
public class IdentityType
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string IdentityTypeName { get; set; } = string.Empty;
}
