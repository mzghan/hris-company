using HRIS.Api.DTOs.Evaluation;
using HRIS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HRIS.Api.Controllers;
[ApiController][Route("api/evaluations")][Authorize]
public class EvaluationsController:ControllerBase
{
 private readonly IEvaluationService _service; public EvaluationsController(IEvaluationService service)=>_service=service;
 [HttpGet] public async Task<IActionResult> Get()=>Ok(await _service.GetAsync(User.ToUserContext()));
 [HttpGet("{id:int}")] public async Task<IActionResult> Get(int id)=>Ok(await _service.GetByIdAsync(id,User.ToUserContext()));
 [HttpPost("{id:int}/entries")] public async Task<IActionResult> Entry(int id,EvaluationEntryCreateDto dto)=>Ok(await _service.AddEntryAsync(id,dto,User.ToUserContext()));
 [HttpPost("{id:int}/scores")] public async Task<IActionResult> Score(int id,EvaluationScoreCreateDto dto)=>Ok(await _service.AddScoreAsync(id,dto,User.ToUserContext()));
}