using HRIS.Api.Controllers;
using HRIS.Api.DTOs.Learning;
using HRIS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRIS.Api.Controllers;

[ApiController]
[Route("api/learning-materials")]
[Authorize]
public class LearningMaterialsController : ControllerBase
{
    private readonly ILearningMaterialService _service;
    public LearningMaterialsController(ILearningMaterialService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<List<LearningMaterialResponseDto>>> GetAll() => Ok(await _service.GetAllAsync());

    [HttpPost]
    [Authorize(Roles = "HR,Support")]
    public async Task<ActionResult<LearningMaterialResponseDto>> Create(LearningMaterialCreateDto dto) => Ok(await _service.CreateAsync(dto, User.ToUserContext()));

    [HttpPut("{id:int}")]
    [Authorize(Roles = "HR,Support")]
    public async Task<ActionResult<LearningMaterialResponseDto>> Update(int id, LearningMaterialCreateDto dto) => Ok(await _service.UpdateAsync(id, dto, User.ToUserContext()));

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "HR,Support")]
    public async Task<IActionResult> Delete(int id) { await _service.DeleteAsync(id, User.ToUserContext()); return NoContent(); }
}
