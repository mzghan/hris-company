using HRIS.Api.DTOs.Leave;
using HRIS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRIS.Api.Controllers;

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

    // Daftar leave request yang sedang menunggu approval dari user yang login
    // (dicocokkan lewat LeaveApproval.ApproverId, hanya relevan untuk Manager/Admin).
    [HttpGet("pending-approval")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<ActionResult<List<LeaveRequestResponseDto>>> GetPendingForApproval()
    {
        var employeeId = User.GetEmployeeId();
        return Ok(await _service.GetPendingForApproverAsync(employeeId));
    }

    [HttpPost("{id:int}/approve")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<ActionResult<LeaveRequestResponseDto>> Approve(int id, LeaveApprovalActionDto dto)
    {
        var approverId = User.GetEmployeeId();
        return Ok(await _service.ApproveAsync(id, approverId, dto));
    }

    [HttpPost("{id:int}/reject")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<ActionResult<LeaveRequestResponseDto>> Reject(int id, LeaveApprovalActionDto dto)
    {
        var approverId = User.GetEmployeeId();
        return Ok(await _service.RejectAsync(id, approverId, dto));
    }
}
