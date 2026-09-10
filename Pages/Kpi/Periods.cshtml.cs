using System.ComponentModel.DataAnnotations;
using HRIS.Api.DTOs.Kpi;
using HRIS.Api.Exceptions;
using HRIS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HRIS.Api.Pages.Kpi;

[Authorize(AuthenticationSchemes = "Cookies", Roles = "Admin")]
public class PeriodsModel : PageModel
{
    private readonly IKpiService _service;

    public PeriodsModel(IKpiService service)
    {
        _service = service;
    }

    public List<KpiPeriodResponseDto> Periods { get; set; } = new();

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required(ErrorMessage = "Nama periode wajib diisi.")]
        public string Name { get; set; } = string.Empty;

        [Range(2000, 2100)]
        public int Year { get; set; } = DateTime.Today.Year;
    }

    public async Task OnGetAsync()
    {
        Periods = (await _service.GetAllPeriodsAsync()).OrderByDescending(p => p.CreatedAt).ToList();
    }

    public async Task<IActionResult> OnPostCreateAsync()
    {
        if (!ModelState.IsValid)
        {
            Periods = await _service.GetAllPeriodsAsync();
            return Page();
        }

        try
        {
            await _service.CreatePeriodAsync(new KpiPeriodCreateDto { Name = Input.Name, Year = Input.Year });
            TempData["Success"] = $"Periode KPI '{Input.Name}' berhasil dibuat.";
        }
        catch (BadRequestException ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostFinalizeAsync(int id)
    {
        try
        {
            await _service.FinalizeAsync(id);
            TempData["Success"] = "Periode KPI berhasil difinalisasi.";
        }
        catch (Exception ex) when (ex is BadRequestException or NotFoundException)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToPage();
    }
}
