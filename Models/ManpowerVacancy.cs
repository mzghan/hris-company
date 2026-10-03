using System.ComponentModel.DataAnnotations;
namespace HRIS.Api.Models;
public class ManpowerVacancy : IAuditable
{
    public int Id { get; set; }
    public int ManpowerRequestId { get; set; }
    public ManpowerRequest? ManpowerRequest { get; set; }
    [Required, MaxLength(30)] public string VacancyCode { get; set; } = string.Empty;
    public int PositionNo { get; set; }
    [MaxLength(30)] public string Status { get; set; } = "Open";
    public DateOnly? ClosedDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
}
