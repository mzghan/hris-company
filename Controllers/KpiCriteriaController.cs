using HRIS.Api.DTOs.Kpi;
using HRIS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRIS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class KpiCriteriaController : ControllerBase
{
    private readonly IKpiService _service;

    public KpiCriteriaController(IKpiService service)
    {
        _service = service;
    }

    [HttpPost]
    [Authorize(Roles = "HR,Support")]
    public async Task<ActionResult<KpiCriteriaResponseDto>> Create(KpiCriteriaCreateDto dto)
    {
        var created = await _service.CreateCriteriaAsync(dto);
        return Ok(created);
    }

    // Manager juga boleh lihat (bukan hanya Admin) supaya tahu konteks
    // bobot tiap kriteria saat meninjau nilai bawahannya.
    [HttpGet]
    [Authorize(Policy = "ManagerOrHR")]
    public async Task<ActionResult<List<KpiCriteriaResponseDto>>> GetAll()
    {
        return Ok(await _service.GetAllCriteriaAsync());
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "HR,Support")]
    public async Task<ActionResult<KpiCriteriaResponseDto>> Update(int id, KpiCriteriaUpdateDto dto)
    {
        return Ok(await _service.UpdateCriteriaAsync(id, dto));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "HR,Support")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteCriteriaAsync(id);
        return NoContent();
    }
}
