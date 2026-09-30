using HRIS.Api.DTOs.Kpi;
using HRIS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRIS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class KpiPeriodsController : ControllerBase
{
    private readonly IKpiService _service;

    public KpiPeriodsController(IKpiService service)
    {
        _service = service;
    }

    [HttpPost]
    [Authorize(Roles = "HR,Support")]
    public async Task<ActionResult<KpiPeriodResponseDto>> Create(KpiPeriodCreateDto dto)
    {
        var created = await _service.CreatePeriodAsync(dto);
        return Ok(created);
    }

    [HttpGet]
    [Authorize(Policy = "ManagerOrHR")]
    public async Task<ActionResult<List<KpiPeriodResponseDto>>> GetAll()
    {
        return Ok(await _service.GetAllPeriodsAsync());
    }

    [HttpGet("{id:int}")]
    [Authorize(Policy = "ManagerOrHR")]
    public async Task<ActionResult<KpiPeriodResponseDto>> GetById(int id)
    {
        return Ok(await _service.GetPeriodByIdAsync(id));
    }

    // Isi (atau isi ulang, selama period belum Finalized) nilai satu
    // employee untuk sejumlah kriteria sekaligus.
    [HttpPost("{id:int}/scores")]
    [Authorize(Roles = "HR,Support")]
    public async Task<ActionResult<List<EmployeeKpiScoreResponseDto>>> FillScores(int id, EmployeeKpiScoreFillDto dto)
    {
        var filledByUserId = User.GetUserId();
        return Ok(await _service.FillScoresAsync(id, filledByUserId, dto));
    }

    // Overview seluruh nilai dalam satu periode (semua employee, semua
    // kriteria) — untuk Admin memantau progres pengisian.
    [HttpGet("{id:int}/scores")]
    [Authorize(Roles = "HR,Support")]
    public async Task<ActionResult<List<EmployeeKpiScoreResponseDto>>> GetScores(int id)
    {
        return Ok(await _service.GetScoresForPeriodAsync(id));
    }

    // Ringkasan nilai + final score satu employee pada periode ini.
    [HttpGet("{id:int}/employees/{employeeId:int}/summary")]
    [Authorize(Policy = "ManagerOrHR")]
    public async Task<ActionResult<EmployeeKpiSummaryDto>> GetEmployeeSummary(int id, int employeeId)
    {
        return Ok(await _service.GetEmployeeSummaryAsync(id, employeeId));
    }

    // Daftar bawahan langsung dari Manager yang login, beserta nilai KPI
    // mereka pada periode ini — dipakai Manager untuk meninjau sebelum
    // memutuskan override atau membiarkan nilai apa adanya.
    [HttpGet("{id:int}/pending-review")]
    [Authorize(Policy = "ManagerOrHR")]
    public async Task<ActionResult<List<EmployeeKpiSummaryDto>>> GetPendingReview(int id)
    {
        var managerEmployeeId = User.GetEmployeeId();
        return Ok(await _service.GetPendingReviewForManagerAsync(id, managerEmployeeId));
    }

    [HttpPost("{id:int}/finalize")]
    [Authorize(Roles = "HR,Support")]
    public async Task<ActionResult<KpiPeriodResponseDto>> Finalize(int id)
    {
        return Ok(await _service.FinalizeAsync(id));
    }
}
