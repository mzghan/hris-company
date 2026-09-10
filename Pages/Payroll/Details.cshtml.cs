using HRIS.Api.Controllers;
using HRIS.Api.DTOs.Payroll;
using HRIS.Api.Exceptions;
using HRIS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HRIS.Api.Pages.Payroll;

[Authorize(AuthenticationSchemes = "Cookies", Roles = "Manager,Admin")]
public class DetailsModel : PageModel
{
    private readonly IPayrollService _service;

    public DetailsModel(IPayrollService service)
    {
        _service = service;
    }

    [BindProperty(SupportsGet = true)]
    public int Id { get; set; }

    public PayrollPeriodResponseDto? Period { get; set; }
    public bool IsAdmin { get; set; }
    public bool CanActOnCurrentLevel { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        IsAdmin = User.IsInRole("Admin");
        try
        {
            Period = await _service.GetByIdAsync(Id);
        }
        catch (NotFoundException)
        {
            TempData["Error"] = "Periode payroll tidak ditemukan.";
            return RedirectToPage("/Payroll/Index");
        }

        if (User.HasClaim(c => c.Type == "employeeId"))
        {
            var employeeId = User.GetEmployeeId();
            CanActOnCurrentLevel = Period.Approvals.Any(a =>
                a.Level == Period.CurrentLevel && a.ApproverId == employeeId && a.Status == "Pending");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostApproveAsync(string? note)
    {
        var approverId = User.GetEmployeeId();
        try
        {
            await _service.ApproveAsync(Id, approverId, new PayrollApprovalActionDto { Note = note });
            TempData["Success"] = "Periode payroll berhasil disetujui.";
        }
        catch (Exception ex) when (ex is ForbiddenException or BadRequestException or NotFoundException)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToPage(new { id = Id });
    }

    public async Task<IActionResult> OnPostRejectAsync(string? note)
    {
        var approverId = User.GetEmployeeId();
        try
        {
            await _service.RejectAsync(Id, approverId, new PayrollApprovalActionDto { Note = note });
            TempData["Success"] = "Periode payroll ditolak.";
        }
        catch (Exception ex) when (ex is ForbiddenException or BadRequestException or NotFoundException)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToPage(new { id = Id });
    }

    public async Task<IActionResult> OnPostMarkPaidAsync()
    {
        if (!User.IsInRole("Admin"))
            return Forbid();

        try
        {
            await _service.MarkPaidAsync(Id);
            TempData["Success"] = "Periode payroll ditandai sudah dibayar.";
        }
        catch (Exception ex) when (ex is BadRequestException or NotFoundException)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToPage(new { id = Id });
    }
}
