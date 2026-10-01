using System.ComponentModel.DataAnnotations;
namespace HRIS.Api.Models;
public class EmployeeEvaluation : IAuditable
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }
    public int EmploymentId { get; set; }
    public EmployeeEmployment? Employment { get; set; }
    public int EvaluationTypeId { get; set; }
    public EvaluationType? EvaluationType { get; set; }
    public DateOnly DueDate { get; set; }
    [Required, MaxLength(30)] public string Status { get; set; } = "Pending";
    public DateTime? CompletedAt { get; set; }
    public ICollection<EvaluationEntry> Entries { get; set; } = new List<EvaluationEntry>();
    public ICollection<EvaluationScore> Scores { get; set; } = new List<EvaluationScore>();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
}