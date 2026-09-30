using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.Models;

// REF_Hierarchy_type. Seed: Direct Manager, Dotted Manager.
public class HierarchyType
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string HierarchyTypeName { get; set; } = string.Empty;
}
