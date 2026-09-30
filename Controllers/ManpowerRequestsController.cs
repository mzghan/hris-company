using HRIS.Api.DTOs.Manpower;
using HRIS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRIS.Api.Controllers;

[ApiController]
[Route("api/manpower-requests")]
[Authorize]
public class ManpowerRequestsController : ControllerBase
{
    private readonly IManpowerService _service;
    public ManpowerRequestsController(IManpowerService service)=>_service=service;

    [HttpGet]
    public async Task<IActionResult> Get()=>Ok(await _service.GetAsync(User.ToUserContext()));

    [HttpPost]
    public async Task<IActionResult> Create(ManpowerRequestCreateDto dto)=>Ok(await _service.CreateAsync(dto,User.ToUserContext()));
}
