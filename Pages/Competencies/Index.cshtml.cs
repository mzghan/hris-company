using HRIS.Api.Controllers;
using HRIS.Api.DTOs.Competency;
using HRIS.Api.DTOs.Employee;
using HRIS.Api.Exceptions;
using HRIS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace HRIS.Api.Pages.Competencies;
[Authorize(AuthenticationSchemes="Cookies")]
public class IndexModel:PageModel
{
 private readonly ICompetencyService _service; private readonly IEmployeeService _employees;
 public IndexModel(ICompetencyService service,IEmployeeService employees){_service=service;_employees=employees;}
 public List<EmployeeCompetencyResponseDto> Items{get;set;}=new(); public List<CompetencyResponseDto> Competencies{get;set;}=new(); public List<EmployeeResponseDto> Employees{get;set;}=new();
 public bool CanAssign=>User.IsInRole("HR")||User.IsInRole("Support")||User.HasClaim("IsManager","true");
 [BindProperty] public int EmployeeId{get;set;} [BindProperty] public int CompetencyId{get;set;} [BindProperty] public int TargetLevel{get;set;}=3;
 [BindProperty] public int EmployeeCompetencyId{get;set;} [BindProperty] public int SelfLevel{get;set;}=3; [BindProperty] public string? Note{get;set;}
 public async Task OnGetAsync(){Items=await _service.GetEmployeeCompetenciesAsync(User.ToUserContext());Competencies=await _service.GetCompetenciesAsync();Employees=await _employees.GetAllAsync();}
 public async Task<IActionResult> OnPostAssignAsync(){try{await _service.AssignAsync(new EmployeeCompetencyCreateDto{EmployeeId=EmployeeId,CompetencyId=CompetencyId,TargetLevel=TargetLevel},User.ToUserContext());TempData["Success"]="Kompetensi ditetapkan.";}catch(Exception ex)when(ex is BadRequestException or ForbiddenException or NotFoundException){TempData["Error"]=ex.Message;}return RedirectToPage();}
 public async Task<IActionResult> OnPostAssessAsync(){try{await _service.AssessAsync(new CompetencyAssessmentCreateDto{EmployeeCompetencyId=EmployeeCompetencyId,SelfLevel=SelfLevel,Note=Note},User.ToUserContext());TempData["Success"]="Assessment disimpan.";}catch(Exception ex)when(ex is BadRequestException or ForbiddenException or NotFoundException){TempData["Error"]=ex.Message;}return RedirectToPage();}
}