using System.ComponentModel.DataAnnotations;
namespace HRIS.Api.Models;
public class EvaluationScore : IAuditable
{
    public int Id { get; set; }
    public int EvaluationId { get; set; }
    public EmployeeEvaluation? Evaluation { get; set; }
    public int ScoredByUserId { get; set; }
    public User? ScoredByUser { get; set; }
    [Range(0,100)] public int Score { get; set; }
    [MaxLength(1000)] public string? Note { get; set; }
    public DateTime ScoredAt { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
}