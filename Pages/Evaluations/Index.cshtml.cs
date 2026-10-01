using HRIS.Api.Controllers;
using HRIS.Api.DTOs.Evaluation;
using HRIS.Api.Exceptions;
using HRIS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace HRIS.Api.Pages.Evaluations;
[Authorize(AuthenticationSchemes="Cookies")]
public class IndexModel:PageModel
{
 private readonly IEvaluationService _service; public IndexModel(IEvaluationService service)=>_service=service;
 public List<EvaluationResponseDto> Items{get;set;}=new();
 [BindProperty] public string EntryText{get;set;}="";
 [BindProperty] public int Score{get;set;}
 [BindProperty] public string? ScoreNote{get;set;}
 public bool CanScore=>User.IsInRole("HR")||User.IsInRole("Support")||User.HasClaim("IsManager","true");
 public async Task OnGetAsync()=>Items=await _service.GetAsync(User.ToUserContext());
 public async Task<IActionResult> OnPostEntryAsync(int id){try{await _service.AddEntryAsync(id,new EvaluationEntryCreateDto{WorkDescription=EntryText},User.ToUserContext());TempData["Success"]="Catatan evaluasi ditambahkan.";}catch(Exception ex)when(ex is BadRequestException or ForbiddenException or NotFoundException){TempData["Error"]=ex.Message;}return RedirectToPage();}
 public async Task<IActionResult> OnPostScoreAsync(int id){try{await _service.AddScoreAsync(id,new EvaluationScoreCreateDto{Score=Score,Note=ScoreNote},User.ToUserContext());TempData["Success"]="Nilai evaluasi disimpan.";}catch(Exception ex)when(ex is BadRequestException or ForbiddenException or NotFoundException){TempData["Error"]=ex.Message;}return RedirectToPage();}
}