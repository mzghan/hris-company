using System.ComponentModel.DataAnnotations;
using HRIS.Api.DTOs.Employee;
using HRIS.Api.Exceptions;
using HRIS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HRIS.Api.Pages.Employees;

[Authorize(AuthenticationSchemes = "Cookies", Roles = "Admin")]
public class EditModel : PageModel
{
    private readonly IEmployeeService _employeeService;
    private readonly IDepartmentService _departmentService;

    public EditModel(IEmployeeService employeeService, IDepartmentService departmentService)
    {
        _employeeService = employeeService;
        _departmentService = departmentService;
    }

    [BindProperty(SupportsGet = true)]
    public int Id { get; set; }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string EmployeeEmail { get; set; } = string.Empty;

    public List<SelectListItem> DepartmentOptions { get; set; } = new();
    public List<SelectListItem> ManagerOptions { get; set; } = new();

    public class InputModel
    {
        [Required(ErrorMessage = "Nama wajib diisi.")]
        public string FullName { get; set; } = string.Empty;

        public string? Position { get; set; }
        public int? DepartmentId { get; set; }
        public int? ManagerId { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        try
        {
            var employee = await _employeeService.GetByIdAsync(Id);
            EmployeeEmail = employee.Email;
            Input = new InputModel
            {
                FullName = employee.FullName,
                Position = employee.Position,
                DepartmentId = employee.DepartmentId,
                ManagerId = employee.ManagerId,
                IsActive = employee.IsActive
            };
        }
        catch (NotFoundException)
        {
            TempData["Error"] = "Karyawan tidak ditemukan.";
            return RedirectToPage("/Employees/Index");
        }

        await LoadOptionsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadOptionsAsync();
            return Page();
        }

        try
        {
            var updated = await _employeeService.UpdateAsync(Id, new EmployeeUpdateDto
            {
                FullName = Input.FullName,
                Position = Input.Position,
                DepartmentId = Input.DepartmentId,
                ManagerId = Input.ManagerId,
                IsActive = Input.IsActive
            });

            TempData["Success"] = $"Data karyawan '{updated.FullName}' berhasil diperbarui.";
            return RedirectToPage("/Employees/Index");
        }
        catch (Exception ex) when (ex is NotFoundException or BadRequestException)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await LoadOptionsAsync();
            return Page();
        }
    }

    private async Task LoadOptionsAsync()
    {
        var departments = await _departmentService.GetAllAsync();
        DepartmentOptions = departments
            .Select(d => new SelectListItem(d.Name, d.Id.ToString()))
            .ToList();

        var employees = await _employeeService.GetAllAsync();
        ManagerOptions = employees
            .Where(e => e.Id != Id)
            .Select(e => new SelectListItem(e.FullName, e.Id.ToString()))
            .ToList();
    }
}
