using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.Models;

// REF_Employment_Status. Seed: Active, Probation, Resigned, Terminated.
public class EmploymentStatus
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string EmploymentStatusName { get; set; } = string.Empty;
}
