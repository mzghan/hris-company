namespace HRIS.Api.DTOs.Performance;
public class PerformancePlanResponseDto
{
 public int Id{get;set;} public int EmployeeId{get;set;} public string EmployeeName{get;set;}="";
 public DateOnly StartDate{get;set;} public DateOnly EndDate{get;set;} public string Status{get;set;}="";
 public int CreatedByUserId{get;set;} public string CreatedByUsername{get;set;}="";
 public List<PlanTaskDto> Tasks{get;set;}=new(); public List<PlanWorkLogDto> WorkLogs{get;set;}=new();
}
public class PlanTaskDto{public int Id{get;set;} public string Title{get;set;}=""; public DateOnly? DueDate{get;set;} public string Status{get;set;}="";}
public class PlanWorkLogDto{public int Id{get;set;} public string Description{get;set;}=""; public DateTime LoggedAt{get;set;}}
public class PerformancePlanCreateDto{public int EmployeeId{get;set;} public DateOnly StartDate{get;set;} public DateOnly EndDate{get;set;}}
public class PlanTaskCreateDto{public int PlanId{get;set;} public string Title{get;set;}=""; public DateOnly? DueDate{get;set;}}
public class PlanTaskStatusDto{public string Status{get;set;}="";}
public class PlanWorkLogCreateDto{public int PlanId{get;set;} public string Description{get;set;}="";}
