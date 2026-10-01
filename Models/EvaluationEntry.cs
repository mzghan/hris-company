using System.ComponentModel.DataAnnotations;
namespace HRIS.Api.Models;
public class EvaluationEntry : IAuditable
{
    public int Id { get; set; }
    public int EvaluationId { get; set; }
    public EmployeeEvaluation? Evaluation { get; set; }
    [Required, MaxLength(2000)] public string WorkDescription { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
}