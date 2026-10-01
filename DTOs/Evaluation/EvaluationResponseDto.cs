namespace HRIS.Api.DTOs.Evaluation;
public class EvaluationResponseDto
{
 public int Id{get;set;} public int EmployeeId{get;set;} public string EmployeeName{get;set;}="";
 public int EmploymentId{get;set;} public int EvaluationTypeId{get;set;} public string EvaluationTypeName{get;set;}="";
 public DateOnly DueDate{get;set;} public string Status{get;set;}=""; public DateTime? CompletedAt{get;set;}
 public List<EvaluationEntryDto> Entries{get;set;}=new(); public List<EvaluationScoreDto> Scores{get;set;}=new();
}
public class EvaluationEntryDto{public int Id{get;set;} public string WorkDescription{get;set;}=""; public DateTime CreatedAt{get;set;}}
public class EvaluationScoreDto{public int Id{get;set;} public int ScoredByUserId{get;set;} public string ScoredByUsername{get;set;}=""; public int Score{get;set;} public string? Note{get;set;} public DateTime ScoredAt{get;set;}}
public class EvaluationEntryCreateDto{public string WorkDescription{get;set;}="";}
public class EvaluationScoreCreateDto{public int Score{get;set;} public string? Note{get;set;}}
