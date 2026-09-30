using HRIS.Api.DTOs.Employee;
using HRIS.Api.Exceptions;
using HRIS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRIS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _service;

    public EmployeesController(IEmployeeService service)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize(Policy = "ManagerOrHR")]
    public async Task<ActionResult<List<EmployeeResponseDto>>> GetAll() =>
        Ok(await _service.GetAllAsync());

    // Employee biasa hanya boleh melihat data dirinya sendiri.
    [HttpGet("{id:int}")]
    public async Task<ActionResult<EmployeeResponseDto>> GetById(int id)
    {
        if (!User.IsHrOrSupport() && !User.IsManager() && User.GetEmployeeId() != id)
            throw new ForbiddenException("Kamu hanya boleh melihat data dirimu sendiri.");

        return Ok(await _service.GetByIdAsync(id));
    }

    [HttpPost]
    [Authorize(Roles = "HR,Support")]
    public async Task<ActionResult<EmployeeResponseDto>> Create(EmployeeCreateDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "HR,Support")]
    public async Task<ActionResult<EmployeeResponseDto>> Update(int id, EmployeeUpdateDto dto) =>
        Ok(await _service.UpdateAsync(id, dto));

    // Mutasi / kenaikan grade / kontrak baru: menutup Employment lama dan membuat yang baru.
    [HttpPut("{id:int}/employment")]
    [Authorize(Roles = "HR,Support")]
    public async Task<ActionResult<EmployeeResponseDto>> ChangeEmployment(int id, EmploymentChangeDto dto) =>
        Ok(await _service.ChangeEmploymentAsync(id, dto));

    [HttpPut("{id:int}/manager")]
    [Authorize(Roles = "HR,Support")]
    public async Task<ActionResult<EmployeeResponseDto>> ChangeManager(int id, ChangeManagerDto dto) =>
        Ok(await _service.ChangeDirectManagerAsync(id, dto));

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "HR,Support")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
