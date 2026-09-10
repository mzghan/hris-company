using System.ComponentModel.DataAnnotations;
using HRIS.Api.DTOs.Employee;
using HRIS.Api.DTOs.Payroll;
using HRIS.Api.Exceptions;
using HRIS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HRIS.Api.Pages.Payroll;

[Authorize(AuthenticationSchemes = "Cookies", Roles = "Admin")]
public class SalariesModel : PageModel
{
    private readonly IEmployeeSalaryService _salaryService;
    private readonly IEmployeeService _employeeService;

    public SalariesModel(IEmployeeSalaryService salaryService, IEmployeeService employeeService)
    {
        _salaryService = salaryService;
        _employeeService = employeeService;
    }

    [BindProperty(SupportsGet = true)]
    public int? EmployeeId { get; set; }

    public List<SelectListItem> EmployeeOptions { get; set; } = new();
    public string? SelectedEmployeeName { get; set; }
    public List<EmployeeSalaryResponseDto> History { get; set; } = new();

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required]
        public int EmployeeId { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Gaji pokok tidak boleh negatif.")]
        public decimal BaseSalary { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Tunjangan tidak boleh negatif.")]
        public decimal AllowanceTotal { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EffectiveDate { get; set; } = DateTime.Today;
    }

    public async Task OnGetAsync()
    {
        await LoadAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            EmployeeId = Input.EmployeeId;
            await LoadAsync();
            return Page();
        }

        try
        {
            await _salaryService.CreateAsync(new EmployeeSalaryCreateDto
            {
                EmployeeId = Input.EmployeeId,
                BaseSalary = Input.BaseSalary,
                AllowanceTotal = Input.AllowanceTotal,
                EffectiveDate = DateOnly.FromDateTime(Input.EffectiveDate)
            });

            TempData["Success"] = "Data gaji berhasil disimpan.";
            return RedirectToPage(new { employeeId = Input.EmployeeId });
        }
        catch (Exception ex) when (ex is BadRequestException or NotFoundException)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            EmployeeId = Input.EmployeeId;
            await LoadAsync();
            return Page();
        }
    }

    private async Task LoadAsync()
    {
        var employees = await _employeeService.GetAllAsync();
        EmployeeOptions = employees.Select(e => new SelectListItem(e.FullName, e.Id.ToString())).ToList();

        if (EmployeeId is not null)
        {
            var emp = employees.FirstOrDefault(e => e.Id == EmployeeId);
            SelectedEmployeeName = emp?.FullName;
            History = (await _salaryService.GetHistoryAsync(EmployeeId.Value))
                .OrderByDescending(s => s.EffectiveDate)
                .ToList();
            Input.EmployeeId = EmployeeId.Value;
        }
    }
}
