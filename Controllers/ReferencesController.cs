using HRIS.Api.DTOs.Reference;
using HRIS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRIS.Api.Controllers;

// Daftar Id + Nama tabel REF_* untuk dropdown, mis. GET /api/references/jobtitle.
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReferencesController : ControllerBase
{
    private readonly IReferenceService _service;

    public ReferencesController(IReferenceService service)
    {
        _service = service;
    }

    [HttpGet("{type}")]
    public async Task<ActionResult<List<ReferenceItemDto>>> GetOptions(string type) =>
        Ok(await _service.GetOptionsAsync(type));
}
