using HRIS.Api.Common;
using HRIS.Api.DTOs.Booking;
using HRIS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRIS.Api.Controllers;

[ApiController]
[Route("api/booking")]
[Authorize]
public class BookingController : ControllerBase
{
    private readonly IBookingService _service;
    public BookingController(IBookingService service)=>_service=service;

    [HttpGet("rooms")]
    public async Task<IActionResult> Rooms()=>Ok(await _service.GetRoomsAsync(User.IsInRole(RoleNames.HR)||User.IsInRole(RoleNames.Support)));

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] DateTime? from,[FromQuery] DateTime? to)=>Ok(await _service.GetBookingsAsync(User.ToUserContext(),from,to));

    [HttpPost]
    public async Task<IActionResult> Create(RoomBookingCreateDto dto)=>Ok(await _service.CreateAsync(dto,User.ToUserContext()));

    [HttpPost("{id:int}/cancel")]
    public async Task<IActionResult> Cancel(int id){await _service.CancelAsync(id,User.ToUserContext());return NoContent();}
}
