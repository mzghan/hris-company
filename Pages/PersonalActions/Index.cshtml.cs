using HRIS.Api.Common;
using HRIS.Api.Controllers;
using HRIS.Api.DTOs.Employee;
using HRIS.Api.DTOs.PersonalAction;
using HRIS.Api.DTOs.Reference;
using HRIS.Api.Exceptions;
using HRIS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace HRIS.Api.Pages.PersonalActions;
[Authorize(AuthenticationSchemes="Cookies")]
public class IndexModel:PageModel
{
 private readonly IPersonalActionService _service; private readonly IEmployeeService _employees; private readonly IReferenceService _refs;
 public IndexModel(IPersonalActionService service,IEmployeeService employees,IReferenceService refs){_service=service;_employees=employees;_refs=refs;}
 public List<PersonalActionResponseDto> Items{get;set;}=new(); public List<EmployeeResponseDto> Employees{get;set;}=new(); public List<ReferenceItemDto> Organizations{get;set;}=new();public List<ReferenceItemDto> JobTitles{get;set;}=new();public List<ReferenceItemDto> JobLevels{get;set;}=new();public List<ReferenceItemDto> Grades{get;set;}=new();public List<ReferenceItemDto> Locations{get;set;}=new();
 public bool IsHrOrSupport=>User.IsInRole("HR")||User.IsInRole("Support");
 [BindProperty] public int EmployeeId{get;set;} [BindProperty] public string ActionType{get;set;}="Transfer"; [BindProperty] public DateTime EffectiveDate{get;set;}=DateTime.Today; [BindProperty] public string Reason{get;set;}="";
 [BindProperty] public int? NewOrganizationId{get;set;} [BindProperty] public int? NewJobTitleId{get;set;} [BindProperty] public int? NewJobLevelId{get;set;} [BindProperty] public int? NewGradeId{get;set;} [BindProperty] public int? NewLocationId{get;set;} [BindProperty] public int? NewManagerId{get;set;}
 public async Task OnGetAsync(){await LoadAsync();}
 public async Task<IActionResult> OnPostAsync(){try{var id=EmployeeId==0?(User.ToUserContext().EmployeeId??0):EmployeeId;await _service.CreateAsync(new PersonalActionCreateDto{EmployeeId=id,ActionType=ActionType,EffectiveDate=DateOnly.FromDateTime(EffectiveDate),Reason=Reason,NewOrganizationId=NewOrganizationId,NewJobTitleId=NewJobTitleId,NewJobLevelId=NewJobLevelId,NewGradeId=NewGradeId,NewLocationId=NewLocationId,NewManagerId=NewManagerId},User.ToUserContext());TempData["Success"]="PAF berhasil dikirim ke approval.";}catch(Exception ex)when(ex is BadRequestException or ForbiddenException or NotFoundException){TempData["Error"]=ex.Message;}return RedirectToPage();}
 private async Task LoadAsync(){Items=await _service.GetAsync(User.ToUserContext());Employees=await _employees.GetAllAsync();Organizations=await _refs.GetOptionsAsync("organization");JobTitles=await _refs.GetOptionsAsync("jobtitle");JobLevels=await _refs.GetOptionsAsync("joblevel");Grades=await _refs.GetOptionsAsync("grade");Locations=await _refs.GetOptionsAsync("location");if(!IsHrOrSupport&&User.ToUserContext().EmployeeId is int id)EmployeeId=id;}
}