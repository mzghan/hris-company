using HRIS.Api.Controllers;
using System.ComponentModel.DataAnnotations;
using HRIS.Api.Common;
using HRIS.Api.DTOs.JobDescription;
using HRIS.Api.DTOs.Reference;
using HRIS.Api.Exceptions;
using HRIS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace HRIS.Api.Pages.JobDescriptions;
[Authorize(AuthenticationSchemes="Cookies")]
public class IndexModel:PageModel
{
    private readonly IJobDescriptionService _service;
    private readonly IReferenceService _refs;
    private readonly IEmployeeService _employees;
    public IndexModel(IJobDescriptionService service,IReferenceService refs,IEmployeeService employees){_service=service;_refs=refs;_employees=employees;}
    public List<JobDescriptionResponseDto> Items {get;set;}=new();
    public List<ReferenceItemDto> Organizations {get;set;}=new();
    public List<ReferenceItemDto> JobLevels {get;set;}=new();
    public List<ReferenceItemDto> Grades {get;set;}=new();
    public List<DTOs.Employee.EmployeeResponseDto> Employees {get;set;}=new();
    public bool CanCreate { get { var actor = User.ToUserContext(); return actor.IsManpowerAdmin || actor.IsManagerRole || actor.IsHead || User.IsManager(); } }
    [BindProperty] public InputModel Input {get;set;}=new();
    public class InputModel {
        public int? OrganizationId{get;set;} public int? JobLevelId{get;set;} public int? GradeId{get;set;} public int? JobHolderEmployeeId{get;set;} public int? ImmediateManagerEmployeeId{get;set;}
        [Required,MaxLength(200)] public string JobTitle{get;set;}="";
        public string Purpose{get;set;}=""; public string ReportingRelationship{get;set;}=""; public string Dimensions{get;set;}="";
        public string KriCico{get;set;}=""; public string KriCompliance{get;set;}=""; public string KriAreas{get;set;}="";
        public string Stakeholders{get;set;}=""; public string Challenges{get;set;}=""; public string Qualifications{get;set;}="";
        public string Experience{get;set;}=""; public string Competencies{get;set;}="";
    }
    [BindProperty(SupportsGet = true)] public string? View { get; set; }
    public async Task OnGetAsync()=>await LoadAsync();
    public async Task<IActionResult> OnPostAsync(bool submit=false){
        if(!ModelState.IsValid){await LoadAsync();return Page();}
        try{await _service.CreateAsync(new JobDescriptionCreateDto{OrganizationId=Input.OrganizationId,JobLevelId=Input.JobLevelId,GradeId=Input.GradeId,JobHolderEmployeeId=Input.JobHolderEmployeeId,ImmediateManagerEmployeeId=Input.ImmediateManagerEmployeeId,JobTitle=Input.JobTitle,Purpose=Input.Purpose,ReportingRelationship=Input.ReportingRelationship,Dimensions=Input.Dimensions,KriCico=Input.KriCico,KriCompliance=Input.KriCompliance,KriAreas=Input.KriAreas,Stakeholders=Input.Stakeholders,Challenges=Input.Challenges,Qualifications=Input.Qualifications,Experience=Input.Experience,Competencies=Input.Competencies},User.ToUserContext(),submit);TempData["Success"]=submit?"JD dikirim ke approval.":"JD disimpan sebagai draft.";return RedirectToPage();}
        catch(Exception ex) when(ex is BadRequestException or ForbiddenException){ModelState.AddModelError("",ex.Message);await LoadAsync();return Page();}
    }
    private async Task LoadAsync(){Items=await _service.GetAsync(User.ToUserContext());Organizations=await _refs.GetOptionsAsync("organization");JobLevels=await _refs.GetOptionsAsync("joblevel");Grades=await _refs.GetOptionsAsync("grade");Employees=await _employees.GetAllAsync();}
}
