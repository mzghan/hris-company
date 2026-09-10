using HRIS.Api.DTOs.Payroll;
using HRIS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRIS.Api.Controllers;

// Master data gaji per employee. Hanya Admin yang boleh mengelola,
// karena ini input langsung untuk perhitungan PayrollItem.
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class EmployeeSalariesController : ControllerBase
{
    private readonly IEmployeeSalaryService _service;

    public EmployeeSalariesController(IEmployeeSalaryService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult<EmployeeSalaryResponseDto>> Create(EmployeeSalaryCreateDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return Ok(created);
    }

    [HttpGet("employee/{employeeId:int}")]
    public async Task<ActionResult<List<EmployeeSalaryResponseDto>>> GetHistory(int employeeId)
    {
        return Ok(await _service.GetHistoryAsync(employeeId));
    }
}
