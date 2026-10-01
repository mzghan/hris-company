namespace HRIS.Api.DTOs.Competency;
public class CompetencyResponseDto{public int Id{get;set;} public string Name{get;set;}=""; public string? Description{get;set;}}
public class EmployeeCompetencyResponseDto
{
 public int Id{get;set;} public int EmployeeId{get;set;} public string EmployeeName{get;set;}="";
 public int CompetencyId{get;set;} public string CompetencyName{get;set;}=""; public int TargetLevel{get;set;}
 public List<CompetencyAssessmentDto> Assessments{get;set;}=new();
}
public class CompetencyAssessmentDto{public int Id{get;set;} public int SelfLevel{get;set;} public string? Note{get;set;} public DateTime AssessedAt{get;set;}}
public class EmployeeCompetencyCreateDto{public int EmployeeId{get;set;} public int CompetencyId{get;set;} public int TargetLevel{get;set;}}
public class CompetencyAssessmentCreateDto{public int EmployeeCompetencyId{get;set;} public int SelfLevel{get;set;} public string? Note{get;set;}}
