using HRIS.Api.DTOs.Audit;
using HRIS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRIS.Api.Controllers;

// Jejak aksi sensitif (terutama aksi Support). Hanya Support yang boleh membaca.
[ApiController]
[Route("api/audit-logs")]
[Authorize(Roles = "Support")]
public class AuditLogsController : ControllerBase
{
    private readonly IAuditLogService _service;

    public AuditLogsController(IAuditLogService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<AuditLogResponseDto>>> GetRecent([FromQuery] int take = 100) =>
        Ok(await _service.GetRecentAsync(take));
}
