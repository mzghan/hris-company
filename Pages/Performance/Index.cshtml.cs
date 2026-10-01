using HRIS.Api.Controllers;
using HRIS.Api.DTOs.Performance;
using HRIS.Api.DTOs.Employee;
using HRIS.Api.Exceptions;
using HRIS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace HRIS.Api.Pages.Performance;
[Authorize(AuthenticationSchemes="Cookies")]
public class IndexModel:PageModel
{
 private readonly IPerformanceService _service; private readonly IEmployeeService _employees;
 public IndexModel(IPerformanceService service,IEmployeeService employees){_service=service;_employees=employees;}
 public List<PerformancePlanResponseDto> Plans{get;set;}=new(); public List<EmployeeResponseDto> Employees{get;set;}=new();
 public bool IsHrOrSupport=>User.IsInRole("HR")||User.IsInRole("Support");
 [BindProperty] public int EmployeeId{get;set;} [BindProperty] public DateTime StartDate{get;set;}=DateTime.Today; [BindProperty] public DateTime EndDate{get;set;}=DateTime.Today.AddMonths(3);
 [BindProperty] public int PlanId{get;set;} [BindProperty] public string TaskTitle{get;set;}=""; [BindProperty] public DateTime? TaskDueDate{get;set;}
 [BindProperty] public string WorkDescription{get;set;}="";
 public async Task OnGetAsync(){Plans=await _service.GetPlansAsync(User.ToUserContext());Employees=await _employees.GetAllAsync();if(!IsHrOrSupport&&User.ToUserContext().EmployeeId is int id)EmployeeId=id;}
 public async Task<IActionResult> OnPostPlanAsync(){try{var id=EmployeeId==0?(User.ToUserContext().EmployeeId??0):EmployeeId;await _service.CreatePlanAsync(new PerformancePlanCreateDto{EmployeeId=id,StartDate=DateOnly.FromDateTime(StartDate),EndDate=DateOnly.FromDateTime(EndDate)},User.ToUserContext());TempData["Success"]="Performance plan dibuat.";}catch(Exception ex)when(ex is BadRequestException or ForbiddenException or NotFoundException){TempData["Error"]=ex.Message;}return RedirectToPage();}
 public async Task<IActionResult> OnPostTaskAsync(){try{await _service.AddTaskAsync(new PlanTaskCreateDto{PlanId=PlanId,Title=TaskTitle,DueDate=TaskDueDate is null?null:DateOnly.FromDateTime(TaskDueDate.Value)},User.ToUserContext());TempData["Success"]="Task ditambahkan.";}catch(Exception ex)when(ex is BadRequestException or ForbiddenException or NotFoundException){TempData["Error"]=ex.Message;}return RedirectToPage();}
 public async Task<IActionResult> OnPostLogAsync(){try{await _service.AddWorkLogAsync(new PlanWorkLogCreateDto{PlanId=PlanId,Description=WorkDescription},User.ToUserContext());TempData["Success"]="Work log ditambahkan.";}catch(Exception ex)when(ex is BadRequestException or ForbiddenException or NotFoundException){TempData["Error"]=ex.Message;}return RedirectToPage();}
 public async Task<IActionResult> OnPostTaskStatusAsync(int id,string status){try{await _service.UpdateTaskStatusAsync(id,new PlanTaskStatusDto{Status=status},User.ToUserContext());TempData["Success"]="Status task diperbarui.";}catch(Exception ex)when(ex is BadRequestException or ForbiddenException or NotFoundException){TempData["Error"]=ex.Message;}return RedirectToPage();}
}