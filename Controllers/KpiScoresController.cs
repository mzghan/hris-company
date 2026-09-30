using HRIS.Api.DTOs.Kpi;
using HRIS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRIS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class KpiScoresController : ControllerBase
{
    private readonly IKpiService _service;

    public KpiScoresController(IKpiService service)
    {
        _service = service;
    }

    // Manager override nilai bawahan langsungnya. Note wajib diisi
    // (divalidasi lewat KpiScoreOverrideDto), tercatat sebagai
    // KpiScoreRevision — nilai lama tidak pernah hilang tanpa jejak.
    [HttpPost("{id:int}/override")]
    [Authorize(Policy = "ManagerOrHR")]
    public async Task<ActionResult<EmployeeKpiScoreResponseDto>> Override(int id, KpiScoreOverrideDto dto)
    {
        var managerEmployeeId = User.GetEmployeeId();
        var revisedByUserId = User.GetUserId();
        return Ok(await _service.OverrideScoreAsync(id, managerEmployeeId, revisedByUserId, dto));
    }
}
