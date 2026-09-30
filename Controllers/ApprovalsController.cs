using HRIS.Api.DTOs.Approval;
using HRIS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRIS.Api.Controllers;

// Endpoint generik approval engine, dipakai semua jenis pengajuan (Leave, dan modul berikutnya).
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ApprovalsController : ControllerBase
{
    private readonly IApprovalService _service;

    public ApprovalsController(IApprovalService service)
    {
        _service = service;
    }

    // Pengajuan yang menunggu keputusan user yang login. all=true (HR/Support): semua yang masih berjalan.
    [HttpGet]
    public async Task<ActionResult<List<ApprovalRequestResponseDto>>> GetInbox([FromQuery] bool all = false) =>
        Ok(await _service.GetInboxAsync(User.ToUserContext(), all));

    // Pengajuan yang dibuat user yang login.
    [HttpGet("me")]
    public async Task<ActionResult<List<ApprovalRequestResponseDto>>> GetMine() =>
        Ok(await _service.GetMyRequestsAsync(User.ToUserContext()));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApprovalRequestResponseDto>> GetById(int id) =>
        Ok(await _service.GetByIdAsync(id, User.ToUserContext()));

    [HttpPost("{id:int}/approve")]
    public async Task<ActionResult<ApprovalRequestResponseDto>> Approve(int id, ApprovalActionDto dto) =>
        Ok(await _service.ApproveAsync(id, User.ToUserContext(), dto.Note));

    // Alasan penolakan (Note) wajib.
    [HttpPost("{id:int}/reject")]
    public async Task<ActionResult<ApprovalRequestResponseDto>> Reject(int id, ApprovalActionDto dto) =>
        Ok(await _service.RejectAsync(id, User.ToUserContext(), dto.Note));

    [HttpPost("{id:int}/cancel")]
    public async Task<ActionResult<ApprovalRequestResponseDto>> Cancel(int id) =>
        Ok(await _service.CancelAsync(id, User.ToUserContext()));

    // --- Konfigurasi alur (REF_Approval_Flow_Step) ---

    [HttpGet("flows")]
    [Authorize(Roles = "HR,Support")]
    public async Task<ActionResult<List<ApprovalFlowStepResponseDto>>> GetFlows() =>
        Ok(await _service.GetFlowStepsAsync());

    [HttpPut("flows/{id:int}")]
    [Authorize(Roles = "HR,Support")]
    public async Task<ActionResult<ApprovalFlowStepResponseDto>> UpdateFlow(int id, ApprovalFlowStepUpdateDto dto) =>
        Ok(await _service.UpdateFlowStepAsync(id, dto));
}
