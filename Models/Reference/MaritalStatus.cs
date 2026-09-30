using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.Models;

// REF_Marital_Status
public class MaritalStatus
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string MaritalStatusName { get; set; } = string.Empty;
}
