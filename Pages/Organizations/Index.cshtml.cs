using System.ComponentModel.DataAnnotations;
using HRIS.Api.DTOs.Organization;
using HRIS.Api.Exceptions;
using HRIS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HRIS.Api.Pages.Organizations;

[Authorize(AuthenticationSchemes = "Cookies", Roles = "HR,Support")]
public class IndexModel : PageModel
{
    private readonly IOrganizationService _service;

    public IndexModel(IOrganizationService service)
    {
        _service = service;
    }

    public List<OrganizationResponseDto> Organizations { get; set; } = new();
    public List<SelectListItem> ParentOptions { get; set; } = new();

    [BindProperty]
    public NewOrganizationInput NewOrganization { get; set; } = new();

    public class NewOrganizationInput
    {
        [Required(ErrorMessage = "Nama organisasi wajib diisi.")]
        public string Name { get; set; } = string.Empty;

        public int? ParentId { get; set; }
    }

    public async Task OnGetAsync()
    {
        await LoadAsync();
    }

    public async Task<IActionResult> OnPostCreateAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadAsync();
            return Page();
        }

        try
        {
            await _service.CreateAsync(new OrganizationCreateDto
            {
                OrganizationName = NewOrganization.Name,
                ParentId = NewOrganization.ParentId
            });
            TempData["Success"] = $"Organisasi '{NewOrganization.Name}' berhasil ditambahkan.";
        }
        catch (BadRequestException ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRenameAsync(int id, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            TempData["Error"] = "Nama organisasi tidak boleh kosong.";
            return RedirectToPage();
        }

        try
        {
            // Rename tidak mengubah induk: ambil ParentId yang sekarang.
            var current = await _service.GetByIdAsync(id);
            await _service.UpdateAsync(id, new OrganizationCreateDto
            {
                OrganizationName = name,
                ParentId = current.ParentId
            });
            TempData["Success"] = "Nama organisasi berhasil diperbarui.";
        }
        catch (Exception ex) when (ex is NotFoundException or BadRequestException)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        try
        {
            await _service.DeleteAsync(id);
            TempData["Success"] = "Organisasi berhasil dihapus.";
        }
        catch (Exception ex) when (ex is NotFoundException or BadRequestException)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToPage();
    }

    private async Task LoadAsync()
    {
        Organizations = await _service.GetAllAsync();
        ParentOptions = Organizations
            .Select(o => new SelectListItem(new string('\u2014', o.OrganizationLevel - 1) + " " + o.OrganizationName, o.Id.ToString()))
            .ToList();
    }
}
