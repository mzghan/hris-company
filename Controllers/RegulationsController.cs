using HRIS.Api.Controllers;
using HRIS.Api.DTOs.Regulation;
using HRIS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRIS.Api.Controllers;

[ApiController]
[Route("api/regulations")]
[Authorize]
public class RegulationsController : ControllerBase
{
    private readonly IRegulationService _service;
    public RegulationsController(IRegulationService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<List<RegulationResponseDto>>> GetAll([FromQuery] bool includeInactive = false) =>
        Ok(await _service.GetAllAsync(includeInactive, User.ToUserContext()));

    [HttpPost]
    [Authorize(Roles = "HR,Support")]
    public async Task<ActionResult<RegulationResponseDto>> Create(RegulationCreateDto dto) => Ok(await _service.CreateAsync(dto, User.ToUserContext()));

    [HttpPut("{id:int}")]
    [Authorize(Roles = "HR,Support")]
    public async Task<ActionResult<RegulationResponseDto>> Update(int id, RegulationCreateDto dto) => Ok(await _service.UpdateAsync(id, dto, User.ToUserContext()));

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "HR,Support")]
    public async Task<IActionResult> Delete(int id) { await _service.DeleteAsync(id, User.ToUserContext()); return NoContent(); }
}
