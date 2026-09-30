using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.Models;

// REF_Employment_Type. Contoh: Permanent, Contract, Outsource.
public class EmploymentType
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string EmploymentTypeName { get; set; } = string.Empty;

    [MaxLength(300)]
    public string? EmploymentTypeDescription { get; set; }
}
