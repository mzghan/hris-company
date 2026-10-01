using HRIS.Api.DTOs.PersonalAction;
using HRIS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HRIS.Api.Controllers;
[ApiController][Route("api/personal-actions")][Authorize]
public class PersonalActionsController:ControllerBase
{
 private readonly IPersonalActionService _service; public PersonalActionsController(IPersonalActionService service)=>_service=service;
 [HttpGet] public async Task<IActionResult> Get()=>Ok(await _service.GetAsync(User.ToUserContext()));
 [HttpPost] public async Task<IActionResult> Create(PersonalActionCreateDto dto)=>Ok(await _service.CreateAsync(dto,User.ToUserContext()));
}