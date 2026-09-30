using HRIS.Api.Controllers;
using HRIS.Api.DTOs.Expatriate;
using HRIS.Api.Exceptions;
using HRIS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HRIS.Api.Pages.Expatriate;

[Authorize(AuthenticationSchemes = "Cookies")]
public class IndexModel : PageModel
{
    private readonly IExpatriateService _service;
    private readonly IReferenceService _references;
    public IndexModel(IExpatriateService service, IReferenceService references) { _service = service; _references = references; }

    [BindProperty(SupportsGet = true)] public int? EmployeeId { get; set; }
    public bool CanManage { get; set; }
    public List<ExpatriateEmployeeResponseDto> Employees { get; set; } = new();
    public ExpatriateEmployeeResponseDto? CurrentEmployee { get; set; }
    public List<SelectListItem> IdentityTypeOptions { get; set; } = new();

    public async Task OnGetAsync()
    {
        var actor = User.ToUserContext();
        CanManage = actor.IsHrOrSupport;
        if (CanManage)
        {
            Employees = await _service.GetExpatriatesAsync(actor);
            if (EmployeeId is not null) CurrentEmployee = await _service.GetAsync(EmployeeId.Value, actor);
        }
        else if (actor.EmployeeId is not null)
        {
            try { CurrentEmployee = await _service.GetAsync(actor.EmployeeId.Value, actor); } catch (NotFoundException) { }
        }
        IdentityTypeOptions = (await _references.GetOptionsAsync("IdentityType"))
            .Select(x => new SelectListItem(x.Name, x.Id.ToString())).ToList();
    }

    public async Task<IActionResult> OnPostAddAsync(int employeeId, int identityTypeId, string identityNumber, DateOnly? validUntil, bool isPrimary)
    {
        try
        {
            await _service.AddIdentityAsync(employeeId, new EmployeeIdentityCreateDto { IdentityTypeId = identityTypeId, IdentityNumber = identityNumber, ValidUntil = validUntil, IsPrimary = isPrimary }, User.ToUserContext());
            TempData["Success"] = "Identitas ditambahkan.";
        }
        catch (Exception ex) when (ex is BadRequestException or ForbiddenException or NotFoundException) { TempData["Error"] = ex.Message; }
        return RedirectToPage(new { employeeId });
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        try { await _service.DeleteIdentityAsync(id, User.ToUserContext()); TempData["Success"] = "Identitas dihapus."; }
        catch (Exception ex) when (ex is ForbiddenException or NotFoundException) { TempData["Error"] = ex.Message; }
        return RedirectToPage();
    }
}
