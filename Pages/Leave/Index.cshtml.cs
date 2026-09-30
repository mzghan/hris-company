using System.ComponentModel.DataAnnotations;
using HRIS.Api.Controllers;
using HRIS.Api.DTOs.Leave;
using HRIS.Api.Exceptions;
using HRIS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HRIS.Api.Pages.Leave;

[Authorize(AuthenticationSchemes = "Cookies", Roles = "Employee")]
public class IndexModel : PageModel
{
    private readonly ILeaveRequestService _service;

    public IndexModel(ILeaveRequestService service)
    {
        _service = service;
    }

    public List<LeaveRequestResponseDto> MyRequests { get; set; } = new();
    public List<HRIS.Api.Models.LeaveType> LeaveTypes { get; set; } = new();
    public List<HRIS.Api.Models.LeaveBalance> Balances { get; set; } = new();
    public int SelectedYear { get; set; }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required(ErrorMessage = "Jenis cuti wajib dipilih.")]
        public int LeaveTypeId { get; set; }

        [Required(ErrorMessage = "Tanggal mulai wajib diisi.")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Tanggal selesai wajib diisi.")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; } = DateTime.Today;

        public string? Reason { get; set; }
    }

    public async Task OnGetAsync()
    {
        var employeeId = User.GetEmployeeId();
        SelectedYear = DateTime.Today.Year;
        LeaveTypes = await _service.GetLeaveTypesAsync();
        Balances = await _service.GetBalancesAsync(employeeId, SelectedYear);
        MyRequests = (await _service.GetMyRequestsAsync(employeeId))
            .OrderByDescending(r => r.CreatedAt)
            .ToList();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var employeeId = User.GetEmployeeId();

        if (!ModelState.IsValid)
        {
            LeaveTypes = await _service.GetLeaveTypesAsync();
            SelectedYear = DateTime.Today.Year;
            Balances = await _service.GetBalancesAsync(employeeId, SelectedYear);
            MyRequests = await _service.GetMyRequestsAsync(employeeId);
            return Page();
        }

        try
        {
            await _service.CreateAsync(employeeId, new LeaveRequestCreateDto
            {
                LeaveTypeId = Input.LeaveTypeId,
                StartDate = DateOnly.FromDateTime(Input.StartDate),
                EndDate = DateOnly.FromDateTime(Input.EndDate),
                Reason = Input.Reason
            });

            TempData["Success"] = "Pengajuan cuti berhasil dikirim.";
            return RedirectToPage();
        }
        catch (Exception ex) when (ex is BadRequestException or ForbiddenException)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            LeaveTypes = await _service.GetLeaveTypesAsync();
            SelectedYear = DateTime.Today.Year;
            Balances = await _service.GetBalancesAsync(employeeId, SelectedYear);
            MyRequests = await _service.GetMyRequestsAsync(employeeId);
            return Page();
        }
    }

    public async Task<IActionResult> OnPostCancelAsync(int id)
    {
        try
        {
            await _service.CancelAsync(id, User.ToUserContext());
            TempData["Success"] = "Pengajuan cuti dibatalkan.";
        }
        catch (Exception ex) when (ex is BadRequestException or ForbiddenException or NotFoundException)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToPage();
    }
}
