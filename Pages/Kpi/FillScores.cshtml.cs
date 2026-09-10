using System.ComponentModel.DataAnnotations;
using HRIS.Api.Controllers;
using HRIS.Api.DTOs.Employee;
using HRIS.Api.DTOs.Kpi;
using HRIS.Api.Exceptions;
using HRIS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HRIS.Api.Pages.Kpi;

[Authorize(AuthenticationSchemes = "Cookies", Roles = "Admin")]
public class FillScoresModel : PageModel
{
    private readonly IKpiService _kpiService;
    private readonly IEmployeeService _employeeService;

    public FillScoresModel(IKpiService kpiService, IEmployeeService employeeService)
    {
        _kpiService = kpiService;
        _employeeService = employeeService;
    }

    [BindProperty(SupportsGet = true)]
    public int PeriodId { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? EmployeeId { get; set; }

    public KpiPeriodResponseDto? Period { get; set; }
    public List<SelectListItem> EmployeeOptions { get; set; } = new();
    public string? SelectedEmployeeName { get; set; }
    public List<KpiCriteriaResponseDto> AllCriteria { get; set; } = new();
    public List<EmployeeKpiScoreResponseDto> ExistingScoresForEmployee { get; set; } = new();
    public List<EmployeeKpiScoreResponseDto> AllScoresForPeriod { get; set; } = new();

    [BindProperty]
    public List<ScoreInput> Scores { get; set; } = new();

    public class ScoreInput
    {
        public int CriteriaId { get; set; }
        public string CriteriaName { get; set; } = string.Empty;

        [Range(0, 100, ErrorMessage = "Skor harus antara 0-100.")]
        public decimal Score { get; set; }
    }

    public async Task<IActionResult> OnGetAsync()
    {
        try
        {
            Period = await _kpiService.GetPeriodByIdAsync(PeriodId);
        }
        catch (NotFoundException)
        {
            TempData["Error"] = "Periode KPI tidak ditemukan.";
            return RedirectToPage("/Kpi/Periods");
        }

        var employees = await _employeeService.GetAllAsync();
        EmployeeOptions = employees.Select(e => new SelectListItem(e.FullName, e.Id.ToString())).ToList();

        AllCriteria = await _kpiService.GetAllCriteriaAsync();
        AllScoresForPeriod = await _kpiService.GetScoresForPeriodAsync(PeriodId);

        if (EmployeeId is not null)
        {
            var emp = employees.FirstOrDefault(e => e.Id == EmployeeId);
            SelectedEmployeeName = emp?.FullName;
            ExistingScoresForEmployee = AllScoresForPeriod.Where(s => s.EmployeeId == EmployeeId).ToList();

            Scores = AllCriteria.Select(c => new ScoreInput
            {
                CriteriaId = c.Id,
                CriteriaName = c.Name,
                Score = ExistingScoresForEmployee.FirstOrDefault(s => s.CriteriaId == c.Id)?.Score ?? 0
            }).ToList();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (EmployeeId is null)
        {
            TempData["Error"] = "Pilih karyawan terlebih dahulu.";
            return RedirectToPage(new { periodId = PeriodId });
        }

        var filledByUserId = User.GetUserId();

        try
        {
            await _kpiService.FillScoresAsync(PeriodId, filledByUserId, new EmployeeKpiScoreFillDto
            {
                EmployeeId = EmployeeId.Value,
                Scores = Scores.Select(s => new KpiScoreItemDto { CriteriaId = s.CriteriaId, Score = s.Score }).ToList()
            });

            TempData["Success"] = "Nilai KPI berhasil disimpan.";
        }
        catch (Exception ex) when (ex is BadRequestException or NotFoundException)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToPage(new { periodId = PeriodId, employeeId = EmployeeId });
    }
}
