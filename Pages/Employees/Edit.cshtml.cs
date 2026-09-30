using System.ComponentModel.DataAnnotations;
using HRIS.Api.DTOs.Employee;
using HRIS.Api.Exceptions;
using HRIS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HRIS.Api.Pages.Employees;

// Hanya data pribadi. Perubahan jabatan/organisasi/grade/lokasi dan perubahan atasan
// menyimpan riwayat, jadi lewat PUT /api/employees/{id}/employment dan /manager.
[Authorize(AuthenticationSchemes = "Cookies", Roles = "HR,Support")]
public class EditModel : PageModel
{
    private readonly IEmployeeService _employeeService;
    private readonly IReferenceService _referenceService;

    public EditModel(IEmployeeService employeeService, IReferenceService referenceService)
    {
        _employeeService = employeeService;
        _referenceService = referenceService;
    }

    [BindProperty(SupportsGet = true)]
    public int Id { get; set; }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string EmployeeNumber { get; set; } = string.Empty;
    public string? WorkEmail { get; set; }

    public List<SelectListItem> GenderOptions { get; set; } = new();
    public List<SelectListItem> ReligionOptions { get; set; } = new();
    public List<SelectListItem> MaritalStatusOptions { get; set; } = new();
    public List<SelectListItem> CountryOptions { get; set; } = new();

    public class InputModel
    {
        [Required(ErrorMessage = "Nama wajib diisi.")]
        public string FullName { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateOnly? BirthDate { get; set; }

        [DataType(DataType.Date)]
        public DateOnly JoinDate { get; set; }

        public int? NationalityCountryId { get; set; }
        public int? ReligionId { get; set; }
        public int? GenderId { get; set; }
        public int? MaritalStatusId { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        try
        {
            var employee = await _employeeService.GetByIdAsync(Id);
            EmployeeNumber = employee.EmployeeNumber;
            WorkEmail = employee.WorkEmail;
            Input = new InputModel
            {
                FullName = employee.FullName,
                BirthDate = employee.BirthDate,
                JoinDate = employee.JoinDate,
                NationalityCountryId = employee.NationalityCountryId,
                ReligionId = employee.ReligionId,
                GenderId = employee.GenderId,
                MaritalStatusId = employee.MaritalStatusId,
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
            await LoadHeaderAsync();
            await LoadOptionsAsync();
            return Page();
        }

        try
        {
            await _employeeService.UpdateAsync(Id, new EmployeeUpdateDto
            {
                FullName = Input.FullName,
                BirthDate = Input.BirthDate,
                JoinDate = Input.JoinDate,
                NationalityCountryId = Input.NationalityCountryId,
                ReligionId = Input.ReligionId,
                GenderId = Input.GenderId,
                MaritalStatusId = Input.MaritalStatusId,
                IsActive = Input.IsActive
            });

            TempData["Success"] = "Data karyawan berhasil diperbarui.";
            return RedirectToPage("/Employees/Index");
        }
        catch (Exception ex) when (ex is NotFoundException or BadRequestException)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await LoadHeaderAsync();
            await LoadOptionsAsync();
            return Page();
        }
    }

    private async Task LoadHeaderAsync()
    {
        try
        {
            var employee = await _employeeService.GetByIdAsync(Id);
            EmployeeNumber = employee.EmployeeNumber;
            WorkEmail = employee.WorkEmail;
        }
        catch (NotFoundException)
        {
            EmployeeNumber = "(tidak ditemukan)";
        }
    }

    private async Task<List<SelectListItem>> OptionsAsync(string type) =>
        (await _referenceService.GetOptionsAsync(type))
            .Select(i => new SelectListItem(i.Name, i.Id.ToString()))
            .ToList();

    private async Task LoadOptionsAsync()
    {
        GenderOptions = await OptionsAsync("gender");
        ReligionOptions = await OptionsAsync("religion");
        MaritalStatusOptions = await OptionsAsync("maritalstatus");
        CountryOptions = await OptionsAsync("country");
    }
}
