using System.ComponentModel.DataAnnotations;
using HRIS.Api.Controllers;
using HRIS.Api.DTOs.Kpi;
using HRIS.Api.Exceptions;
using HRIS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HRIS.Api.Pages.Kpi;

[Authorize(AuthenticationSchemes = "Cookies", Policy = "ManagerOrHR")]
public class ReviewDetailModel : PageModel
{
    private readonly IKpiService _service;

    public ReviewDetailModel(IKpiService service)
    {
        _service = service;
    }

    [BindProperty(SupportsGet = true)]
    public int PeriodId { get; set; }

    [BindProperty(SupportsGet = true)]
    public int EmployeeId { get; set; }

    public EmployeeKpiSummaryDto? Summary { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        try
        {
            Summary = await _service.GetEmployeeSummaryAsync(PeriodId, EmployeeId);
        }
        catch (NotFoundException)
        {
            TempData["Error"] = "Data KPI tidak ditemukan.";
            return RedirectToPage("/Kpi/Review", new { periodId = PeriodId });
        }
        return Page();
    }

    public async Task<IActionResult> OnPostOverrideAsync(int scoreId, decimal newScore, [Required(ErrorMessage = "Catatan alasan wajib diisi.")] string note)
    {
        if (string.IsNullOrWhiteSpace(note))
        {
            TempData["Error"] = "Catatan alasan wajib diisi saat override nilai.";
            return RedirectToPage(new { periodId = PeriodId, employeeId = EmployeeId });
        }

        var managerEmployeeId = User.GetEmployeeId();
        var revisedByUserId = User.GetUserId();

        try
        {
            await _service.OverrideScoreAsync(scoreId, managerEmployeeId, revisedByUserId, new KpiScoreOverrideDto
            {
                NewScore = newScore,
                Note = note
            });
            TempData["Success"] = "Nilai berhasil di-override dan tercatat di riwayat.";
        }
        catch (Exception ex) when (ex is ForbiddenException or BadRequestException or NotFoundException)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToPage(new { periodId = PeriodId, employeeId = EmployeeId });
    }
}
