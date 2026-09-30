using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.Models;

// REF_Module_Eligibility. Aturan kelayakan modul berdasarkan employment type.
public class ModuleEligibility
{
    public int Id { get; set; }
    public int EmploymentTypeId { get; set; }
    public EmploymentType? EmploymentType { get; set; }
    [Required, MaxLength(30)] public string ModuleCode { get; set; } = string.Empty;
    public bool IsAllowed { get; set; }
}
