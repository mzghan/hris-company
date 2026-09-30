using HRIS.Api.Controllers;
using System.ComponentModel.DataAnnotations;
using HRIS.Api.Common;
using HRIS.Api.DTOs.Manpower;
using HRIS.Api.DTOs.Reference;
using HRIS.Api.Exceptions;
using HRIS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HRIS.Api.Pages.Manpower;

[Authorize(AuthenticationSchemes = "Cookies")]
public class IndexModel : PageModel
{
    private readonly IManpowerService _service;
    private readonly IReferenceService _refs;
    public IndexModel(IManpowerService service,IReferenceService refs){_service=service;_refs=refs;}
    public List<ManpowerRequestResponseDto> Requests { get; set; } = new();
    public List<ReferenceItemDto> Organizations { get; set; } = new();
    public List<ReferenceItemDto> JobTitles { get; set; } = new();
    public List<ReferenceItemDto> JobLevels { get; set; } = new();
    public List<ReferenceItemDto> EmploymentTypes { get; set; } = new();
    public bool CanSubmit => User.IsInRole(RoleNames.HR)||User.IsInRole(RoleNames.Support)||User.IsManager();

    [BindProperty] public InputModel Input { get; set; } = new();
    public class InputModel
    {
        [Required] public int OrganizationId { get; set; }
        [Required] public int JobTitleId { get; set; }
        [Required] public int JobLevelId { get; set; }
        [Required] public int EmploymentTypeId { get; set; }
        [Range(1,1000)] public int Headcount { get; set; } = 1;
        [Required,MaxLength(1000)] public string Reason { get; set; } = string.Empty;
        [Required] public DateTime TargetDate { get; set; } = DateTime.Today.AddDays(30);
    }
    public async Task OnGetAsync()=>await LoadAsync();
    public async Task<IActionResult> OnPostAsync()
    {
        if(!ModelState.IsValid){await LoadAsync();return Page();}
        try
        {
            await _service.CreateAsync(new ManpowerRequestCreateDto
            {OrganizationId=Input.OrganizationId,JobTitleId=Input.JobTitleId,JobLevelId=Input.JobLevelId,EmploymentTypeId=Input.EmploymentTypeId,Headcount=Input.Headcount,Reason=Input.Reason,TargetDate=DateOnly.FromDateTime(Input.TargetDate)},User.ToUserContext());
            TempData["Success"]="Manpower request berhasil dikirim ke approval.";
            return RedirectToPage();
        }catch(Exception ex) when(ex is BadRequestException or ForbiddenException){ModelState.AddModelError("",ex.Message);await LoadAsync();return Page();}
    }
    private async Task LoadAsync()
    {
        Requests=await _service.GetAsync(User.ToUserContext());
        Organizations=await _refs.GetOptionsAsync("organization");
        JobTitles=await _refs.GetOptionsAsync("jobtitle");
        JobLevels=await _refs.GetOptionsAsync("joblevel");
        EmploymentTypes=await _refs.GetOptionsAsync("employmenttype");
    }
}
