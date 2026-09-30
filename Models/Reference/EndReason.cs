using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.Models;

// REF_End_Reason. Contoh: resigned better position, terminated not perform.
public class EndReason
{
    public int Id { get; set; }

    [Required, MaxLength(150)]
    public string EndReasonName { get; set; } = string.Empty;
}
