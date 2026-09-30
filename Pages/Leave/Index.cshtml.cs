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

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
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
        MyRequests = (await _service.GetMyRequestsAsync(employeeId))
            .OrderByDescending(r => r.CreatedAt)
            .ToList();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var employeeId = User.GetEmployeeId();

        if (!ModelState.IsValid)
        {
            MyRequests = await _service.GetMyRequestsAsync(employeeId);
            return Page();
        }

        try
        {
            await _service.CreateAsync(employeeId, new LeaveRequestCreateDto
            {
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
            MyRequests = await _service.GetMyRequestsAsync(employeeId);
            return Page();
        }
    }
}
