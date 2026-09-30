using HRIS.Api.DTOs.Leave;
using HRIS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRIS.Api.Controllers;

// Persetujuan cuti (approve/reject) ada di ApprovalsController: /api/approvals/{id}/approve|reject.
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LeaveRequestsController : ControllerBase
{
    private readonly ILeaveRequestService _service;

    public LeaveRequestsController(ILeaveRequestService service)
    {
        _service = service;
    }

    // Employee mengajukan cuti untuk dirinya sendiri.
    [HttpPost]
    public async Task<ActionResult<LeaveRequestResponseDto>> Create(LeaveRequestCreateDto dto)
    {
        var employeeId = User.GetEmployeeId();
        var created = await _service.CreateAsync(employeeId, dto);
        return Ok(created);
    }

    [HttpGet("me")]
    public async Task<ActionResult<List<LeaveRequestResponseDto>>> GetMyRequests()
    {
        var employeeId = User.GetEmployeeId();
        return Ok(await _service.GetMyRequestsAsync(employeeId));
    }

    // Pengaju membatalkan cuti yang masih Pending.
    [HttpPost("{id:int}/cancel")]
    public async Task<ActionResult<LeaveRequestResponseDto>> Cancel(int id) =>
        Ok(await _service.CancelAsync(id, User.ToUserContext()));
}
