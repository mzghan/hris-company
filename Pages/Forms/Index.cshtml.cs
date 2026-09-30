using HRIS.Api.Controllers;
using HRIS.Api.DTOs.Document;
using HRIS.Api.Exceptions;
using HRIS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HRIS.Api.Pages.Forms;

[Authorize(AuthenticationSchemes = "Cookies")]
[RequestSizeLimit(26_214_400)]
public class IndexModel : PageModel
{
    private readonly IDocumentService _documents;
    public IndexModel(IDocumentService documents) => _documents = documents;

    public List<DocumentResponseDto> Documents { get; set; } = new();
    public List<SelectListItem> FormCategoryOptions { get; set; } = new();
    public bool CanManage { get; set; }

    public async Task OnGetAsync() => await LoadAsync();

    public async Task<IActionResult> OnPostAsync(IFormFile? file, string? title, int categoryId)
    {
        if (file is null || file.Length == 0) { TempData["Error"] = "Pilih file formulir."; return RedirectToPage(); }
        try
        {
            await using var stream = file.OpenReadStream();
            await _documents.UploadAsync(User.ToUserContext(), categoryId,
                string.IsNullOrWhiteSpace(title) ? Path.GetFileNameWithoutExtension(file.FileName) : title,
                null, file.FileName, file.ContentType, file.Length, stream);
            TempData["Success"] = "Formulir berhasil diunggah.";
        }
        catch (Exception ex) when (ex is BadRequestException or ForbiddenException) { TempData["Error"] = ex.Message; }
        return RedirectToPage();
    }

    private async Task LoadAsync()
    {
        var actor = User.ToUserContext();
        CanManage = actor.IsHrOrSupport;
        var categories = await _documents.GetCategoriesAsync();
        var formCategories = categories.Where(c => c.FullName.Equals("Forms", StringComparison.OrdinalIgnoreCase) || c.FullName.StartsWith("Forms > ", StringComparison.OrdinalIgnoreCase)).ToList();
        FormCategoryOptions = formCategories.Select(c => new SelectListItem(c.FullName, c.Id.ToString())).ToList();
        var all = await _documents.GetListAsync(actor);
        Documents = all.Where(d => d.CategoryName.Equals("Forms", StringComparison.OrdinalIgnoreCase) || d.CategoryName.StartsWith("Forms > ", StringComparison.OrdinalIgnoreCase)).ToList();
    }
}
