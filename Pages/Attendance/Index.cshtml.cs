using HRIS.Api.Common;
using HRIS.Api.Controllers;
using HRIS.Api.DTOs.Attendance;
using HRIS.Api.DTOs.Reference;
using HRIS.Api.Exceptions;
using HRIS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HRIS.Api.Pages.Attendance;

[Authorize(AuthenticationSchemes = "Cookies", Roles = "Employee")]
public class IndexModel : PageModel
{
    private readonly IAttendanceService _service;
    private readonly IReferenceService _references;

    public IndexModel(IAttendanceService service, IReferenceService references)
    {
        _service = service;
        _references = references;
    }

    public List<AttendanceResponseDto> History { get; set; } = new();
    public AttendanceResponseDto? Today { get; set; }
    public List<ReferenceItemDto> WorkTypes { get; set; } = new();

    // Tanggal "hari ini" versi Jakarta (WIB), dipakai di header halaman.
    public DateOnly TodayDate { get; set; }

    // Diisi lewat hidden input oleh JS kamera/geolocation di halaman,
    // tepat sebelum form check-in/check-out di-submit.
    [BindProperty]
    public string? PhotoBase64 { get; set; }

    [BindProperty]
    public double? Latitude { get; set; }

    [BindProperty]
    public double? Longitude { get; set; }

    public async Task OnGetAsync()
    {
        var employeeId = User.GetEmployeeId();
        History = (await _service.GetHistoryAsync(employeeId))
            .OrderByDescending(a => a.Date)
            .ToList();

        TodayDate = JakartaTime.Today();
        WorkTypes = await _references.GetOptionsAsync("worktype");
        Today = History.FirstOrDefault(a => a.Date == TodayDate);
    }

    public async Task<IActionResult> OnPostCheckInAsync()
    {
        var employeeId = User.GetEmployeeId();
        try
        {
            await _service.CheckInAsync(employeeId, new AttendanceCheckInDto
            {
                PhotoBase64 = PhotoBase64,
                Latitude = Latitude,
                Longitude = Longitude,
                WorkTypeId = int.TryParse(Request.Form["WorkTypeId"], out var wt) ? wt : 0,
                Note = Request.Form["Note"]
            });
            TempData["Success"] = "Check-in berhasil dicatat.";
        }
        catch (BadRequestException ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostCheckOutAsync()
    {
        var employeeId = User.GetEmployeeId();
        try
        {
            await _service.CheckOutAsync(employeeId, new AttendanceCheckOutDto
            {
                PhotoBase64 = PhotoBase64,
                Latitude = Latitude,
                Longitude = Longitude
            });
            TempData["Success"] = "Check-out berhasil dicatat.";
        }
        catch (BadRequestException ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToPage();
    }
}
