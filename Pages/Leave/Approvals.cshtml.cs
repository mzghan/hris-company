using HRIS.Api.Controllers;
using HRIS.Api.DTOs.Leave;
using HRIS.Api.Exceptions;
using HRIS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HRIS.Api.Pages.Leave;

[Authorize(AuthenticationSchemes = "Cookies", Roles = "Manager,Admin")]
public class ApprovalsModel : PageModel
{
    private readonly ILeaveRequestService _service;

    public ApprovalsModel(ILeaveRequestService service)
    {
        _service = service;
    }

    public List<LeaveRequestResponseDto> Pending { get; set; } = new();

    // Halaman ini boleh diakses Manager & Admin (lihat [Authorize] di atas),
    // tapi tidak semua akun Admin terhubung ke data Employee (User.EmployeeId
    // nullable — lihat catatan di ClaimsPrincipalExtensions). Kalau akun yang
    // login tidak terhubung ke Employee, itu bukan error: cukup tampilkan
    // "tidak ada approval" ke akun tsb, daripada membiarkan exception lolos
    // ke ExceptionHandlingMiddleware dan keluar sebagai JSON mentah 403.
    public bool AccountNotLinkedToEmployee { get; set; }

    public async Task OnGetAsync()
    {
        try
        {
            var approverId = User.GetEmployeeId();
            Pending = await _service.GetPendingForApproverAsync(approverId);
        }
        catch (ForbiddenException)
        {
            AccountNotLinkedToEmployee = true;
        }
    }

    public async Task<IActionResult> OnPostApproveAsync(int id, string? note)
    {
        try
        {
            var approverId = User.GetEmployeeId();
            await _service.ApproveAsync(id, approverId, new LeaveApprovalActionDto { Note = note });
            TempData["Success"] = "Cuti berhasil disetujui.";
        }
        catch (Exception ex) when (ex is ForbiddenException or BadRequestException or NotFoundException)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRejectAsync(int id, string? note)
    {
        try
        {
            var approverId = User.GetEmployeeId();
            await _service.RejectAsync(id, approverId, new LeaveApprovalActionDto { Note = note });
            TempData["Success"] = "Cuti berhasil ditolak.";
        }
        catch (Exception ex) when (ex is ForbiddenException or BadRequestException or NotFoundException)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToPage();
    }
}
