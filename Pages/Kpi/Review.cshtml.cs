using HRIS.Api.Controllers;
using HRIS.Api.DTOs.Kpi;
using HRIS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HRIS.Api.Pages.Kpi;

[Authorize(AuthenticationSchemes = "Cookies", Policy = "ManagerOrHR")]
public class ReviewModel : PageModel
{
    private readonly IKpiService _service;

    public ReviewModel(IKpiService service)
    {
        _service = service;
    }

    [BindProperty(SupportsGet = true)]
    public int? PeriodId { get; set; }

    public List<SelectListItem> PeriodOptions { get; set; } = new();
    public string? SelectedPeriodName { get; set; }
    public List<EmployeeKpiSummaryDto> PendingReview { get; set; } = new();

    public async Task OnGetAsync()
    {
        var periods = await _service.GetAllPeriodsAsync();
        PeriodOptions = periods
            .OrderByDescending(p => p.Year)
            .Select(p => new SelectListItem($"{p.Name} ({p.Status})", p.Id.ToString()))
            .ToList();

        if (PeriodId is not null)
        {
            var period = periods.FirstOrDefault(p => p.Id == PeriodId);
            SelectedPeriodName = period?.Name;

            var managerEmployeeId = User.GetEmployeeId();
            PendingReview = await _service.GetPendingReviewForManagerAsync(PeriodId.Value, managerEmployeeId);
        }
    }
}
