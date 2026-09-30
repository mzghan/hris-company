using System.ComponentModel.DataAnnotations;
using HRIS.Api.Models.Enums;

namespace HRIS.Api.Models;

// TRX_Assistance_Request. employee_id sengaja nullable untuk EAP anonim.
public class AssistanceRequest : IAuditable
{
    public int Id { get; set; }

    public int? EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    [Required, MaxLength(100)]
    public string Category { get; set; } = string.Empty;

    [Required, MaxLength(4000)]
    public string Description { get; set; } = string.Empty;

    public bool IsAnonymous { get; set; }

    [MaxLength(30)]
    public string Status { get; set; } = AssistanceRequestStatus.Pending;

    public int? HandledByUserId { get; set; }
    public User? HandledByUser { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
}
