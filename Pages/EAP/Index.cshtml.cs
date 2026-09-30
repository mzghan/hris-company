using HRIS.Api.Controllers;
using HRIS.Api.DTOs.Assistance;
using HRIS.Api.Exceptions;
using HRIS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HRIS.Api.Pages.EAP;

[Authorize(AuthenticationSchemes = "Cookies")]
public class IndexModel : PageModel
{
    private readonly IAssistanceRequestService _service;
    public IndexModel(IAssistanceRequestService service) => _service = service;
    public List<AssistanceRequestResponseDto> Items { get; set; } = new();
    public bool CanManage { get; set; }

    public async Task OnGetAsync()
    {
        var actor = User.ToUserContext();
        CanManage = actor.IsHrOrSupport;
        Items = await _service.GetListAsync(actor);
    }

    public async Task<IActionResult> OnPostCreateAsync(string category, string description, bool isAnonymous)
    {
        try
        {
            await _service.CreateAsync(new AssistanceRequestCreateDto { Category = category, Description = description, IsAnonymous = isAnonymous }, User.ToUserContext());
            TempData["Success"] = isAnonymous ? "Permintaan EAP anonim berhasil dikirim." : "Permintaan EAP berhasil dikirim ke HR.";
        }
        catch (Exception ex) when (ex is BadRequestException or ForbiddenException) { TempData["Error"] = ex.Message; }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostStatusAsync(int id, string status)
    {
        try { await _service.UpdateStatusAsync(id, status, User.ToUserContext()); TempData["Success"] = "Status EAP diperbarui."; }
        catch (Exception ex) when (ex is BadRequestException or ForbiddenException or NotFoundException) { TempData["Error"] = ex.Message; }
        return RedirectToPage();
    }
}
