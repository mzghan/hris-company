using HRIS.Api.Controllers;
using HRIS.Api.DTOs.Assistance;
using HRIS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRIS.Api.Controllers;

[ApiController]
[Route("api/assistance-requests")]
[Authorize]
public class AssistanceRequestsController : ControllerBase
{
    private readonly IAssistanceRequestService _service;
    public AssistanceRequestsController(IAssistanceRequestService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<List<AssistanceRequestResponseDto>>> GetList() => Ok(await _service.GetListAsync(User.ToUserContext()));

    [HttpPost]
    public async Task<ActionResult<AssistanceRequestResponseDto>> Create(AssistanceRequestCreateDto dto) => Ok(await _service.CreateAsync(dto, User.ToUserContext()));

    [HttpPut("{id:int}/status")]
    [Authorize(Roles = "HR,Support")]
    public async Task<ActionResult<AssistanceRequestResponseDto>> UpdateStatus(int id, AssistanceStatusUpdateDto dto) => Ok(await _service.UpdateStatusAsync(id, dto.Status, User.ToUserContext()));
}
