using HRIS.Api.DTOs.Attendance;
using HRIS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRIS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AttendanceController : ControllerBase
{
    private readonly IAttendanceService _service;

    public AttendanceController(IAttendanceService service)
    {
        _service = service;
    }

    [HttpPost("check-in")]
    public async Task<ActionResult<AttendanceResponseDto>> CheckIn([FromBody] AttendanceCheckInDto dto)
    {
        var employeeId = User.GetEmployeeId();
        return Ok(await _service.CheckInAsync(employeeId, dto));
    }

    [HttpPost("check-out")]
    public async Task<ActionResult<AttendanceResponseDto>> CheckOut([FromBody] AttendanceCheckOutDto dto)
    {
        var employeeId = User.GetEmployeeId();
        return Ok(await _service.CheckOutAsync(employeeId, dto));
    }

    [HttpGet("me")]
    public async Task<ActionResult<List<AttendanceResponseDto>>> GetMyHistory()
    {
        var employeeId = User.GetEmployeeId();
        return Ok(await _service.GetHistoryAsync(employeeId));
    }
}
