using System.ComponentModel.DataAnnotations;
using HRIS.Api.DTOs.Department;
using HRIS.Api.DTOs.Employee;
using HRIS.Api.Exceptions;
using HRIS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HRIS.Api.Pages.Employees;

[Authorize(AuthenticationSchemes = "Cookies", Roles = "Admin")]
public class CreateModel : PageModel
{
    private readonly IEmployeeService _employeeService;
    private readonly IDepartmentService _departmentService;

    public CreateModel(IEmployeeService employeeService, IDepartmentService departmentService)
    {
        _employeeService = employeeService;
        _departmentService = departmentService;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public List<SelectListItem> DepartmentOptions { get; set; } = new();
    public List<SelectListItem> ManagerOptions { get; set; } = new();

    public class InputModel
    {
        [Required(ErrorMessage = "Nama wajib diisi.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email wajib diisi."), EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string? Position { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime HireDate { get; set; } = DateTime.Today;

        public int? DepartmentId { get; set; }
        public int? ManagerId { get; set; }
    }

    public async Task OnGetAsync()
    {
        await LoadOptionsAsync();
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
            await _employeeService.CreateAsync(new EmployeeCreateDto
            {
                FullName = Input.FullName,
                Email = Input.Email,
                Position = Input.Position,
                HireDate = Input.HireDate,
                DepartmentId = Input.DepartmentId,
                ManagerId = Input.ManagerId
            });

            TempData["Success"] = $"Karyawan '{Input.FullName}' berhasil ditambahkan.";
            return RedirectToPage("/Employees/Index");
        }
        catch (BadRequestException ex)
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
            .Select(e => new SelectListItem(e.FullName, e.Id.ToString()))
            .ToList();
    }
}
