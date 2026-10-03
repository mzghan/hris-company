using HRIS.Api.Controllers;
using HRIS.Api.Common;
using HRIS.Api.Exceptions;
using HRIS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HRIS.Api.Pages.JobDescriptions;

[Authorize(AuthenticationSchemes = "Cookies")]
public class SignModel : PageModel
{
    private readonly IJobDescriptionService _service;
    public SignModel(IJobDescriptionService service) => _service = service;

    public int Id { get; set; }
    public string RoleMode { get; set; } = "job-holder";
    public string JobTitle { get; set; } = "";
    public string Code { get; set; } = "";
    public string? ExistingSignature { get; set; }

    [BindProperty] public string Signature { get; set; } = "";

    public async Task<IActionResult> OnGetAsync(int id, string? mode)
    {
        try
        {
            Id = id;
            RoleMode = string.Equals(mode, "manager", StringComparison.OrdinalIgnoreCase) ? "manager" : "job-holder";
            var item = await _service.GetByIdAsync(id, User.ToUserContext());
            JobTitle = item.JobTitle; Code = item.Code;
            ExistingSignature = RoleMode == "manager" ? item.ManagerSignatureText : item.JobHolderSignatureText;
            return Page();
        }
        catch (NotFoundException) { return NotFound(); }
        catch (ForbiddenException ex) { TempData["Error"] = ex.Message; return RedirectToPage("/JobDescriptions/Index"); }
    }

    public async Task<IActionResult> OnPostAsync(int id, string mode)
    {
        if (string.IsNullOrWhiteSpace(Signature))
        {
            ModelState.AddModelError(nameof(Signature), "Tanda tangan wajib diisi.");
            return await OnGetAsync(id, mode);
        }
        try
        {
            if (string.Equals(mode, "manager", StringComparison.OrdinalIgnoreCase))
                await _service.SignManagerAsync(id, Signature, User.ToUserContext());
            else
                await _service.SignJobHolderAsync(id, Signature, User.ToUserContext());
            TempData["Success"] = "JD berhasil ditandatangani.";
            return RedirectToPage("/JobDescriptions/Index", new { view = "submitted" });
        }
        catch (Exception ex) when (ex is BadRequestException or ForbiddenException)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return await OnGetAsync(id, mode);
        }
    }
}
