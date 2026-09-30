using HRIS.Api.Controllers;
using System.ComponentModel.DataAnnotations;
using HRIS.Api.Common;
using HRIS.Api.DTOs.Booking;
using HRIS.Api.DTOs.Reference;
using HRIS.Api.Exceptions;
using HRIS.Api.Models;
using HRIS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HRIS.Api.Pages.Booking;

[Authorize(AuthenticationSchemes = "Cookies")]
public class IndexModel : PageModel
{
    private readonly IBookingService _service;
    private readonly IReferenceService _refs;
    public IndexModel(IBookingService service,IReferenceService refs){_service=service;_refs=refs;}
    public List<MeetingRoom> Rooms { get; set; } = new();
    public List<RoomBookingResponseDto> Bookings { get; set; } = new();
    public List<ReferenceItemDto> Locations { get; set; } = new();
    public bool CanManage => User.IsInRole(RoleNames.HR)||User.IsInRole(RoleNames.Support);
    public bool CanBook => int.TryParse(User.FindFirst("employeeId")?.Value,out _);
    public int? CurrentEmployeeId => int.TryParse(User.FindFirst("employeeId")?.Value,out var id) ? id : null;

    [BindProperty] public BookingInput Input { get; set; } = new();
    [BindProperty] public RoomInput Room { get; set; } = new();

    public class BookingInput
    {
        [Required] public int RoomId { get; set; }
        [Required,MaxLength(200)] public string Title { get; set; } = string.Empty;
        [Required] public DateTime StartTime { get; set; } = DateTime.Now.AddHours(1);
        [Required] public DateTime EndTime { get; set; } = DateTime.Now.AddHours(2);
        public bool IsPriority { get; set; }
    }
    public class RoomInput
    {
        [Required,MaxLength(120)] public string Name { get; set; } = string.Empty;
        [Range(1,1000)] public int Capacity { get; set; } = 8;
        public int LocationId { get; set; }
    }

    public async Task OnGetAsync()=>await LoadAsync();

    public async Task<IActionResult> OnPostBookAsync()
    {
        if(!ModelState.IsValid){await LoadAsync();return Page();}
        try
        {
            await _service.CreateAsync(new RoomBookingCreateDto{RoomId=Input.RoomId,Title=Input.Title,StartTime=Input.StartTime,EndTime=Input.EndTime,IsPriority=Input.IsPriority},User.ToUserContext());
            TempData["Success"]="Meeting room berhasil dibooking.";
            return RedirectToPage();
        }catch(Exception ex) when(ex is BadRequestException or ForbiddenException){ModelState.AddModelError("",ex.Message);await LoadAsync();return Page();}
    }
    public async Task<IActionResult> OnPostCancelAsync(int id)
    {
        try{await _service.CancelAsync(id,User.ToUserContext());TempData["Success"]="Booking dibatalkan.";}
        catch(Exception ex) when(ex is BadRequestException or ForbiddenException or NotFoundException){TempData["Error"]=ex.Message;}
        return RedirectToPage();
    }
    public async Task<IActionResult> OnPostCreateRoomAsync()
    {
        if(!ModelState.IsValid){await LoadAsync();return Page();}
        try{await _service.CreateRoomAsync(Room.Name,Room.Capacity,Room.LocationId,User.ToUserContext());TempData["Success"]="Ruang meeting ditambahkan.";return RedirectToPage();}
        catch(Exception ex) when(ex is BadRequestException or ForbiddenException){ModelState.AddModelError("",ex.Message);await LoadAsync();return Page();}
    }
    public async Task<IActionResult> OnPostToggleRoomAsync(int id,bool active)
    {
        try{await _service.SetRoomActiveAsync(id,active,User.ToUserContext());TempData["Success"]="Status ruang diperbarui.";}
        catch(Exception ex) when(ex is BadRequestException or ForbiddenException or NotFoundException){TempData["Error"]=ex.Message;}
        return RedirectToPage();
    }
    private async Task LoadAsync()
    {
        Rooms=await _service.GetRoomsAsync(CanManage);
        Locations=await _refs.GetOptionsAsync("location");
        var from=DateTime.Today;
        Bookings=await _service.GetBookingsAsync(User.ToUserContext(),from,from.AddDays(14));
    }
}
