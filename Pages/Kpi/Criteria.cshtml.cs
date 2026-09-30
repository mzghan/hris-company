using System.ComponentModel.DataAnnotations;
using HRIS.Api.DTOs.Kpi;
using HRIS.Api.Exceptions;
using HRIS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HRIS.Api.Pages.Kpi;

[Authorize(AuthenticationSchemes = "Cookies", Roles = "HR,Support")]
public class CriteriaModel : PageModel
{
    private readonly IKpiService _service;

    public CriteriaModel(IKpiService service)
    {
        _service = service;
    }

    public List<KpiCriteriaResponseDto> Criteria { get; set; } = new();
    public decimal TotalWeight => Criteria.Sum(c => c.Weight);

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required(ErrorMessage = "Nama kriteria wajib diisi.")]
        public string Name { get; set; } = string.Empty;

        [Range(0, 100, ErrorMessage = "Bobot harus antara 0-100.")]
        public decimal Weight { get; set; }
    }

    public async Task OnGetAsync()
    {
        Criteria = await _service.GetAllCriteriaAsync();
    }

    public async Task<IActionResult> OnPostCreateAsync()
    {
        if (!ModelState.IsValid)
        {
            Criteria = await _service.GetAllCriteriaAsync();
            return Page();
        }

        await _service.CreateCriteriaAsync(new KpiCriteriaCreateDto { Name = Input.Name, Weight = Input.Weight });
        TempData["Success"] = $"Kriteria '{Input.Name}' berhasil ditambahkan.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostUpdateAsync(int id, string name, decimal weight)
    {
        try
        {
            await _service.UpdateCriteriaAsync(id, new KpiCriteriaUpdateDto { Name = name, Weight = weight });
            TempData["Success"] = "Kriteria berhasil diperbarui.";
        }
        catch (NotFoundException ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        try
        {
            await _service.DeleteCriteriaAsync(id);
            TempData["Success"] = "Kriteria berhasil dihapus.";
        }
        catch (NotFoundException ex)
        {
            TempData["Error"] = ex.Message;
        }
        catch (Exception)
        {
            TempData["Error"] = "Kriteria ini tidak bisa dihapus karena sudah dipakai di penilaian.";
        }
        return RedirectToPage();
    }
}
