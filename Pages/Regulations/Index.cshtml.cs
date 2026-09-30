using HRIS.Api.Controllers;
using HRIS.Api.DTOs.Regulation;
using HRIS.Api.Exceptions;
using HRIS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HRIS.Api.Pages.Regulations;

[Authorize(AuthenticationSchemes = "Cookies")]
public class IndexModel : PageModel
{
    private readonly IRegulationService _service;
    public IndexModel(IRegulationService service) => _service = service;
    public List<RegulationResponseDto> Items { get; set; } = new();
    public bool CanManage { get; set; }

    public async Task OnGetAsync()
    {
        var actor = User.ToUserContext();
        CanManage = actor.IsHrOrSupport;
        Items = await _service.GetAllAsync(CanManage, actor);
    }

    public async Task<IActionResult> OnPostSaveAsync(int id, string title, string content, int sortOrder)
    {
        try
        {
            var dto = new RegulationCreateDto { Title = title, Content = content, SortOrder = sortOrder };
            if (id == 0) await _service.CreateAsync(dto, User.ToUserContext());
            else await _service.UpdateAsync(id, dto, User.ToUserContext());
            TempData["Success"] = "Peraturan disimpan.";
        }
        catch (Exception ex) when (ex is BadRequestException or ForbiddenException or NotFoundException) { TempData["Error"] = ex.Message; }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        try { await _service.DeleteAsync(id, User.ToUserContext()); TempData["Success"] = "Peraturan dinonaktifkan."; }
        catch (Exception ex) when (ex is ForbiddenException or NotFoundException) { TempData["Error"] = ex.Message; }
        return RedirectToPage();
    }
}
