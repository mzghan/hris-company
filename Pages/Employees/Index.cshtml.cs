using HRIS.Api.DTOs.Employee;
using HRIS.Api.Exceptions;
using HRIS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HRIS.Api.Pages.Employees;

[Authorize(AuthenticationSchemes = "Cookies", Roles = "HR,Support")]
public class IndexModel : PageModel
{
    private readonly IEmployeeService _service;

    public IndexModel(IEmployeeService service)
    {
        _service = service;
    }

    public List<EmployeeResponseDto> Employees { get; set; } = new();

    public async Task OnGetAsync()
    {
        Employees = await _service.GetAllAsync();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        try
        {
            await _service.DeleteAsync(id);
            TempData["Success"] = "Karyawan berhasil dihapus.";
        }
        catch (Exception ex) when (ex is NotFoundException or BadRequestException)
        {
            TempData["Error"] = ex.Message;
        }
        catch (Exception)
        {
            TempData["Error"] = "Karyawan ini tidak bisa dihapus karena masih punya data terkait (absensi, cuti, dll). Nonaktifkan saja lewat menu Edit.";
        }
        return RedirectToPage();
    }
}
