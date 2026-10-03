using System.ComponentModel.DataAnnotations;
namespace HRIS.Api.Models;
public class ManpowerRequestRemark : IAuditable
{
    public int Id { get; set; }
    public int ManpowerRequestId { get; set; }
    public ManpowerRequest? ManpowerRequest { get; set; }
    public int? ApprovalLevel { get; set; }
    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }
    [Required, MaxLength(2000)] public string Remark { get; set; } = string.Empty;
    [MaxLength(30)] public string Action { get; set; } = "Comment";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
}
