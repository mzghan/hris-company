using System.ComponentModel.DataAnnotations;
using HRIS.Api.Controllers;
using HRIS.Api.DTOs.Payroll;
using HRIS.Api.Exceptions;
using HRIS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HRIS.Api.Pages.Payroll;

[Authorize(AuthenticationSchemes = "Cookies", Roles = "Manager,Admin")]
public class IndexModel : PageModel
{
    private readonly IPayrollService _service;

    public IndexModel(IPayrollService service)
    {
        _service = service;
    }

    public bool IsAdmin { get; set; }
    public List<PayrollPeriodResponseDto> Periods { get; set; } = new();
    public List<PayrollPeriodResponseDto> PendingForMe { get; set; } = new();

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Range(1, 12)]
        public int Month { get; set; } = DateTime.Today.Month;

        [Range(2000, 2100)]
        public int Year { get; set; } = DateTime.Today.Year;
    }

    public async Task OnGetAsync()
    {
        IsAdmin = User.IsInRole("Admin");
        Periods = (await _service.GetAllAsync()).OrderByDescending(p => p.Year).ThenByDescending(p => p.Month).ToList();

        if (User.HasClaim(c => c.Type == "employeeId"))
        {
            var employeeId = User.GetEmployeeId();
            PendingForMe = await _service.GetPendingForApproverAsync(employeeId);
        }
    }

    public async Task<IActionResult> OnPostCreateAsync()
    {
        if (!User.IsInRole("Admin"))
            return Forbid();

        try
        {
            var created = await _service.CreatePeriodAsync(new PayrollPeriodCreateDto
            {
                Month = Input.Month,
                Year = Input.Year
            });

            TempData["Success"] = $"Periode payroll {Input.Month}/{Input.Year} berhasil dibuat.";
            return RedirectToPage("/Payroll/Details", new { id = created.Id });
        }
        catch (BadRequestException ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToPage();
        }
    }
}
