using System.ComponentModel.DataAnnotations;
using HRIS.Api.DTOs.Auth;
using HRIS.Api.Exceptions;
using HRIS.Api.Models.Enums;
using HRIS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HRIS.Api.Pages.Employees;

// Halaman frontend untuk endpoint POST /api/auth/register (Admin only).
// Dipakai Admin untuk membuatkan akun login (Employee/Manager) yang
// terhubung ke data Employee yang sudah ada.
[Authorize(AuthenticationSchemes = "Cookies", Roles = "Admin")]
public class CreateAccountModel : PageModel
{
    private readonly IAuthService _authService;
    private readonly IEmployeeService _employeeService;

    public CreateAccountModel(IAuthService authService, IEmployeeService employeeService)
    {
        _authService = authService;
        _employeeService = employeeService;
    }

    [BindProperty(SupportsGet = true)]
    public int EmployeeId { get; set; }

    public string EmployeeName { get; set; } = string.Empty;

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required(ErrorMessage = "Username wajib diisi."), MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password wajib diisi."), MinLength(6, ErrorMessage = "Password minimal 6 karakter.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public UserRole Role { get; set; } = UserRole.Employee;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        try
        {
            var employee = await _employeeService.GetByIdAsync(EmployeeId);
            EmployeeName = employee.FullName;
        }
        catch (NotFoundException)
        {
            TempData["Error"] = "Karyawan tidak ditemukan.";
            return RedirectToPage("/Employees/Index");
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadEmployeeNameAsync();
            return Page();
        }

        try
        {
            await _authService.RegisterAsync(new RegisterDto
            {
                Username = Input.Username,
                Password = Input.Password,
                Role = Input.Role,
                EmployeeId = EmployeeId
            });

            TempData["Success"] = $"Akun '{Input.Username}' berhasil dibuat untuk karyawan ini.";
            return RedirectToPage("/Employees/Index");
        }
        catch (BadRequestException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await LoadEmployeeNameAsync();
            return Page();
        }
    }

    private async Task LoadEmployeeNameAsync()
    {
        try
        {
            var employee = await _employeeService.GetByIdAsync(EmployeeId);
            EmployeeName = employee.FullName;
        }
        catch (NotFoundException)
        {
            EmployeeName = "(tidak ditemukan)";
        }
    }
}
