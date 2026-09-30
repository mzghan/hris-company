using HRIS.Api.Controllers;
using HRIS.Api.DTOs.Document;
using HRIS.Api.Exceptions;
using HRIS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HRIS.Api.Pages.Documents;

[Authorize(AuthenticationSchemes = "Cookies")]
[RequestSizeLimit(26_214_400)]
public class IndexModel : PageModel
{
    private readonly IDocumentService _service;
    private readonly IEmployeeService _employeeService;

    public IndexModel(IDocumentService service, IEmployeeService employeeService)
    {
        _service = service;
        _employeeService = employeeService;
    }

    [BindProperty(SupportsGet = true)]
    public int? CategoryId { get; set; }

    public List<DocumentResponseDto> Documents { get; set; } = new();
    public List<DocumentCategoryResponseDto> Categories { get; set; } = new();
    public List<SelectListItem> CategoryOptions { get; set; } = new();
    public List<SelectListItem> EmployeeOptions { get; set; } = new();
    public bool CanManage { get; set; }

    public async Task OnGetAsync()
    {
        await LoadAsync();
    }

    public async Task<IActionResult> OnGetDownloadAsync(int id)
    {
        try
        {
            var download = await _service.OpenAsync(id, User.ToUserContext());
            return File(download.Content, download.ContentType, download.FileName);
        }
        catch (Exception ex) when (ex is ForbiddenException or NotFoundException)
        {
            TempData["Error"] = ex.Message;
            return RedirectToPage();
        }
    }

    // Parameter (bukan BindProperty) supaya form upload dan form kategori tidak saling memvalidasi.
    public async Task<IActionResult> OnPostUploadAsync(IFormFile? file, string? title, int categoryId, int? ownerEmployeeId)
    {
        if (file is null || file.Length == 0)
        {
            TempData["Error"] = "Pilih file yang akan diunggah.";
            return RedirectToPage();
        }

        try
        {
            await using var stream = file.OpenReadStream();
            await _service.UploadAsync(
                User.ToUserContext(), categoryId,
                string.IsNullOrWhiteSpace(title) ? Path.GetFileNameWithoutExtension(file.FileName) : title,
                ownerEmployeeId, file.FileName, file.ContentType, file.Length, stream);
            TempData["Success"] = "Dokumen berhasil diunggah.";
        }
        catch (Exception ex) when (ex is BadRequestException or ForbiddenException)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostAddCategoryAsync(string? name, int? parentId)
    {
        if (!User.IsHrOrSupport())
        {
            TempData["Error"] = "Hanya HR/Support yang boleh mengelola kategori.";
            return RedirectToPage();
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            TempData["Error"] = "Nama kategori wajib diisi.";
            return RedirectToPage();
        }

        try
        {
            await _service.CreateCategoryAsync(new DocumentCategoryCreateDto { Name = name, ParentId = parentId });
            TempData["Success"] = "Kategori ditambahkan.";
        }
        catch (BadRequestException ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        try
        {
            await _service.DeleteAsync(id, User.ToUserContext());
            TempData["Success"] = "Dokumen dihapus.";
        }
        catch (Exception ex) when (ex is ForbiddenException or NotFoundException)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToPage();
    }

    private async Task LoadAsync()
    {
        var actor = User.ToUserContext();
        CanManage = actor.IsHrOrSupport;

        Categories = await _service.GetCategoriesAsync();
        CategoryOptions = Categories
            .Select(c => new SelectListItem(c.FullName, c.Id.ToString()))
            .ToList();

        Documents = await _service.GetListAsync(actor, CategoryId);

        if (CanManage)
        {
            EmployeeOptions = (await _employeeService.GetAllAsync())
                .Select(e => new SelectListItem($"{e.FullName} ({e.EmployeeNumber})", e.Id.ToString()))
                .ToList();
        }
    }
}
