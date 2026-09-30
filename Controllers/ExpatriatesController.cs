using HRIS.Api.Controllers;
using HRIS.Api.DTOs.Expatriate;
using HRIS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRIS.Api.Controllers;

[ApiController]
[Route("api/expatriates")]
[Authorize]
public class ExpatriatesController : ControllerBase
{
    private readonly IExpatriateService _service;
    public ExpatriatesController(IExpatriateService service) => _service = service;

    [HttpGet]
    [Authorize(Roles = "HR,Support")]
    public async Task<ActionResult<List<ExpatriateEmployeeResponseDto>>> GetAll() => Ok(await _service.GetExpatriatesAsync(User.ToUserContext()));

    [HttpGet("{employeeId:int}")]
    public async Task<ActionResult<ExpatriateEmployeeResponseDto>> Get(int employeeId) => Ok(await _service.GetAsync(employeeId, User.ToUserContext()));

    [HttpPost("{employeeId:int}/identities")]
    public async Task<ActionResult<EmployeeIdentityResponseDto>> AddIdentity(int employeeId, EmployeeIdentityCreateDto dto) => Ok(await _service.AddIdentityAsync(employeeId, dto, User.ToUserContext()));

    [HttpPut("identities/{id:int}")]
    public async Task<ActionResult<EmployeeIdentityResponseDto>> UpdateIdentity(int id, EmployeeIdentityCreateDto dto) => Ok(await _service.UpdateIdentityAsync(id, dto, User.ToUserContext()));

    [HttpDelete("identities/{id:int}")]
    public async Task<IActionResult> DeleteIdentity(int id) { await _service.DeleteIdentityAsync(id, User.ToUserContext()); return NoContent(); }
}
