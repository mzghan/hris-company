namespace HRIS.Api.DTOs.PersonalAction;
public class PersonalActionResponseDto
{
 public int Id{get;set;} public int EmployeeId{get;set;} public string EmployeeName{get;set;}="";
 public string ActionType{get;set;}=""; public DateOnly EffectiveDate{get;set;} public string Reason{get;set;}="";
 public string Status{get;set;}=""; public int? NewOrganizationId{get;set;} public string? NewOrganizationName{get;set;}
 public int? NewJobTitleId{get;set;} public string? NewJobTitleName{get;set;} public int? NewJobLevelId{get;set;} public string? NewJobLevelName{get;set;}
 public int? NewGradeId{get;set;} public int? NewGradeLevel{get;set;} public int? NewLocationId{get;set;} public string? NewLocationName{get;set;}
 public int? NewManagerId{get;set;} public string? NewManagerName{get;set;} public int? ApprovalId{get;set;}
}
public class PersonalActionCreateDto
{
 public int EmployeeId{get;set;} public string ActionType{get;set;}=""; public DateOnly EffectiveDate{get;set;} public string Reason{get;set;}="";
 public int? NewOrganizationId{get;set;} public int? NewJobTitleId{get;set;} public int? NewJobLevelId{get;set;} public int? NewGradeId{get;set;} public int? NewLocationId{get;set;} public int? NewManagerId{get;set;}
}
