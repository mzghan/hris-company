using HRIS.Api.DTOs.Performance;
using HRIS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HRIS.Api.Controllers;
[ApiController][Route("api/performance")][Authorize]
public class PerformanceController:ControllerBase
{
 private readonly IPerformanceService _service; public PerformanceController(IPerformanceService service)=>_service=service;
 [HttpGet("plans")] public async Task<IActionResult> Plans()=>Ok(await _service.GetPlansAsync(User.ToUserContext()));
 [HttpPost("plans")] public async Task<IActionResult> Create(PerformancePlanCreateDto dto)=>Ok(await _service.CreatePlanAsync(dto,User.ToUserContext()));
 [HttpPost("tasks")] public async Task<IActionResult> TaskCreate(PlanTaskCreateDto dto)=>Ok(await _service.AddTaskAsync(dto,User.ToUserContext()));
 [HttpPost("worklogs")] public async Task<IActionResult> WorkLog(PlanWorkLogCreateDto dto)=>Ok(await _service.AddWorkLogAsync(dto,User.ToUserContext()));
 [HttpPut("tasks/{id:int}/status")] public async Task<IActionResult> Status(int id,PlanTaskStatusDto dto)=>Ok(await _service.UpdateTaskStatusAsync(id,dto,User.ToUserContext()));
}