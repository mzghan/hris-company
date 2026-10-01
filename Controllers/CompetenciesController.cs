using HRIS.Api.DTOs.Competency;
using HRIS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HRIS.Api.Controllers;
[ApiController][Route("api/competencies")][Authorize]
public class CompetenciesController:ControllerBase
{
 private readonly ICompetencyService _service; public CompetenciesController(ICompetencyService service)=>_service=service;
 [HttpGet] public async Task<IActionResult> Get()=>Ok(await _service.GetCompetenciesAsync());
 [HttpGet("employees")] public async Task<IActionResult> Employees()=>Ok(await _service.GetEmployeeCompetenciesAsync(User.ToUserContext()));
 [HttpPost("employees")] public async Task<IActionResult> Assign(EmployeeCompetencyCreateDto dto)=>Ok(await _service.AssignAsync(dto,User.ToUserContext()));
 [HttpPost("employees/assessments")] public async Task<IActionResult> Assess(CompetencyAssessmentCreateDto dto)=>Ok(await _service.AssessAsync(dto,User.ToUserContext()));
}