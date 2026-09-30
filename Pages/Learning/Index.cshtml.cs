using HRIS.Api.Controllers;
using HRIS.Api.DTOs.Document;
using HRIS.Api.DTOs.Learning;
using HRIS.Api.Exceptions;
using HRIS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HRIS.Api.Pages.Learning;

[Authorize(AuthenticationSchemes = "Cookies")]
public class IndexModel : PageModel
{
    private readonly ILearningMaterialService _service;
    private readonly IDocumentService _documentService;

    public IndexModel(ILearningMaterialService service, IDocumentService documentService)
    { _service = service; _documentService = documentService; }

    public List<LearningMaterialResponseDto> Items { get; set; } = new();
    public List<SelectListItem> LearningDocumentOptions { get; set; } = new();
    public bool CanManage { get; set; }

    public async Task OnGetAsync() => await LoadAsync();

    public async Task<IActionResult> OnPostSaveAsync(int id, string title, string? description, int documentId)
    {
        try
        {
            var dto = new LearningMaterialCreateDto { Title = title, Description = description, DocumentId = documentId };
            if (id == 0) await _service.CreateAsync(dto, User.ToUserContext());
            else await _service.UpdateAsync(id, dto, User.ToUserContext());
            TempData["Success"] = "Materi pembelajaran disimpan.";
        }
        catch (Exception ex) when (ex is BadRequestException or ForbiddenException or NotFoundException)
        { TempData["Error"] = ex.Message; }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        try { await _service.DeleteAsync(id, User.ToUserContext()); TempData["Success"] = "Materi dinonaktifkan."; }
        catch (Exception ex) when (ex is ForbiddenException or NotFoundException) { TempData["Error"] = ex.Message; }
        return RedirectToPage();
    }

    private async Task LoadAsync()
    {
        var actor = User.ToUserContext();
        CanManage = actor.IsHrOrSupport;
        Items = await _service.GetAllAsync();
        if (!CanManage) return;
        var categories = await _documentService.GetCategoriesAsync();
        var learning = categories.FirstOrDefault(c => c.FullName.Equals("Learning", StringComparison.OrdinalIgnoreCase));
        if (learning is null) return;
        var documents = await _documentService.GetListAsync(actor, learning.Id);
        LearningDocumentOptions = documents.Select(d => new SelectListItem(d.Title, d.Id.ToString())).ToList();
    }
}
