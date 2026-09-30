using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.Models;

// REF_Relationship. Seed: Spouse, Child, dst.
public class Relationship
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string RelationshipName { get; set; } = string.Empty;
}
