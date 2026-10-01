using System.ComponentModel.DataAnnotations;
namespace HRIS.Api.Models;
public class EmployeeCompetency : IAuditable
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }
    public int CompetencyId { get; set; }
    public Competency? Competency { get; set; }
    [Range(1,5)] public int TargetLevel { get; set; }
    public int AssignedByUserId { get; set; }
    public User? AssignedByUser { get; set; }
    public ICollection<CompetencyAssessment> Assessments { get; set; } = new List<CompetencyAssessment>();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
}