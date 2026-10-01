using System.ComponentModel.DataAnnotations;
namespace HRIS.Api.Models;
public class CompetencyAssessment : IAuditable
{
    public int Id { get; set; }
    public int EmployeeCompetencyId { get; set; }
    public EmployeeCompetency? EmployeeCompetency { get; set; }
    [Range(1,5)] public int SelfLevel { get; set; }
    [MaxLength(1000)] public string? Note { get; set; }
    public DateTime AssessedAt { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
}