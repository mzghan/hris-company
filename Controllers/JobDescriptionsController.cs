using HRIS.Api.Common;
using HRIS.Api.DTOs.JobDescription;
using HRIS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HRIS.Api.Controllers;
[ApiController,Route("api/job-descriptions"),Authorize]
public class JobDescriptionsController:ControllerBase
{
    private readonly IJobDescriptionService _service;
    public JobDescriptionsController(IJobDescriptionService service)=>_service=service;
    [HttpGet] public async Task<IActionResult> Get()=>Ok(await _service.GetAsync(User.ToUserContext()));
    [HttpGet("{id}")] public async Task<IActionResult> GetById(int id)=>Ok(await _service.GetByIdAsync(id,User.ToUserContext()));
    [HttpPost] public async Task<IActionResult> Create(JobDescriptionCreateDto dto,[FromQuery] bool submit=false)=>Ok(await _service.CreateAsync(dto,User.ToUserContext(),submit));
    [HttpPost("{id}/sign-job-holder")] public async Task<IActionResult> SignJobHolder(int id,[FromBody] SignatureDto dto)=>Ok(await _service.SignJobHolderAsync(id,dto.Signature,User.ToUserContext()));
    [HttpPost("{id}/sign-manager")] public async Task<IActionResult> SignManager(int id,[FromBody] SignatureDto dto)=>Ok(await _service.SignManagerAsync(id,dto.Signature,User.ToUserContext()));
    public record SignatureDto(string Signature);
}
