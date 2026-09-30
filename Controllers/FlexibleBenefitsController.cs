using HRIS.Api.DTOs.FlexibleBenefit;
using HRIS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRIS.Api.Controllers;

[ApiController]
[Route("api/flexible-benefits")]
[Authorize]
public class FlexibleBenefitsController : ControllerBase
{
    private readonly IFlexibleBenefitService _service;
    public FlexibleBenefitsController(IFlexibleBenefitService service)=>_service=service;

    [HttpGet("balances")]
    public async Task<IActionResult> Balances([FromQuery] int? year)=>Ok(await _service.GetBalancesAsync(User.ToUserContext(),year??DateTime.Today.Year));

    [HttpGet("periods")]
    public async Task<IActionResult> Periods()=>Ok(await _service.GetPeriodsAsync());

    [HttpGet("encashments")]
    public async Task<IActionResult> Encashments()=>Ok(await _service.GetEncashmentsAsync(User.ToUserContext()));

    [HttpPost("encashments")]
    public async Task<IActionResult> CreateEncashment(LeaveEncashmentCreateDto dto)=>Ok(await _service.CreateEncashmentAsync(dto,User.ToUserContext()));

    [HttpPost("health-claims/{id:int}/status")]
    public async Task<IActionResult> UpdateHealthClaimStatus(int id, [FromQuery] string status){await _service.UpdateHealthClaimStatusAsync(id,status,User.ToUserContext());return NoContent();}

    [HttpGet("health-claims")]
    public async Task<IActionResult> HealthClaims()=>Ok(await _service.GetHealthClaimsAsync(User.ToUserContext()));

    [HttpPost("health-claims")]
    public async Task<IActionResult> CreateHealthClaim(HealthClaimCreateDto dto)=>Ok(await _service.CreateHealthClaimAsync(dto,User.ToUserContext()));
}
