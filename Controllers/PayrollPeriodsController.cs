using HRIS.Api.DTOs.Payroll;
using HRIS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRIS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PayrollPeriodsController : ControllerBase
{
    private readonly IPayrollService _service;

    public PayrollPeriodsController(IPayrollService service)
    {
        _service = service;
    }

    // Admin membuat periode baru: generate PayrollItem untuk tiap employee
    // aktif + PayrollApproval untuk tiap level approver terkonfigurasi.
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PayrollPeriodResponseDto>> Create(PayrollPeriodCreateDto dto)
    {
        var created = await _service.CreatePeriodAsync(dto);
        return Ok(created);
    }

    [HttpGet]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<ActionResult<List<PayrollPeriodResponseDto>>> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<ActionResult<PayrollPeriodResponseDto>> GetById(int id)
    {
        return Ok(await _service.GetByIdAsync(id));
    }

    // Daftar periode yang sedang menunggu approval dari user yang login
    // (dicocokkan lewat PayrollApproval.ApproverId).
    [HttpGet("pending-approval")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<ActionResult<List<PayrollPeriodResponseDto>>> GetPendingForApproval()
    {
        var employeeId = User.GetEmployeeId();
        return Ok(await _service.GetPendingForApproverAsync(employeeId));
    }

    [HttpPost("{id:int}/approve")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<ActionResult<PayrollPeriodResponseDto>> Approve(int id, PayrollApprovalActionDto dto)
    {
        var approverId = User.GetEmployeeId();
        return Ok(await _service.ApproveAsync(id, approverId, dto));
    }

    [HttpPost("{id:int}/reject")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<ActionResult<PayrollPeriodResponseDto>> Reject(int id, PayrollApprovalActionDto dto)
    {
        var approverId = User.GetEmployeeId();
        return Ok(await _service.RejectAsync(id, approverId, dto));
    }

    // Ditandai Paid setelah proses pembayaran aktual di luar sistem ini
    // dilakukan — hanya pencatatan status, tidak ada integrasi pembayaran.
    [HttpPost("{id:int}/mark-paid")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PayrollPeriodResponseDto>> MarkPaid(int id)
    {
        return Ok(await _service.MarkPaidAsync(id));
    }
}
