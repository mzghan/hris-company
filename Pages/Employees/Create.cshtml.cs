using System.ComponentModel.DataAnnotations;
using HRIS.Api.DTOs.Employee;
using HRIS.Api.DTOs.Reference;
using HRIS.Api.Exceptions;
using HRIS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HRIS.Api.Pages.Employees;

[Authorize(AuthenticationSchemes = "Cookies", Roles = "HR,Support")]
public class CreateModel : PageModel
{
    private readonly IEmployeeService _employeeService;
    private readonly IOrganizationService _organizationService;
    private readonly IReferenceService _referenceService;

    public CreateModel(
        IEmployeeService employeeService,
        IOrganizationService organizationService,
        IReferenceService referenceService)
    {
        _employeeService = employeeService;
        _organizationService = organizationService;
        _referenceService = referenceService;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public List<SelectListItem> GenderOptions { get; set; } = new();
    public List<SelectListItem> ReligionOptions { get; set; } = new();
    public List<SelectListItem> MaritalStatusOptions { get; set; } = new();
    public List<SelectListItem> CountryOptions { get; set; } = new();
    public List<SelectListItem> EmploymentTypeOptions { get; set; } = new();
    public List<SelectListItem> EmploymentStatusOptions { get; set; } = new();
    public List<SelectListItem> VendorOptions { get; set; } = new();
    public List<SelectListItem> OrganizationOptions { get; set; } = new();
    public List<SelectListItem> LocationOptions { get; set; } = new();
    public List<SelectListItem> JobLevelOptions { get; set; } = new();
    public List<SelectListItem> JobTitleOptions { get; set; } = new();
    public List<SelectListItem> GradeOptions { get; set; } = new();
    public List<SelectListItem> ManagerOptions { get; set; } = new();

    public class InputModel
    {
        [Required(ErrorMessage = "Nomor karyawan wajib diisi.")]
        public string EmployeeNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nama wajib diisi.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email kerja wajib diisi."), EmailAddress]
        public string WorkEmail { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateOnly? BirthDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateOnly JoinDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

        public int? NationalityCountryId { get; set; }
        public int? ReligionId { get; set; }
        public int? GenderId { get; set; }
        public int? MaritalStatusId { get; set; }

        [Required(ErrorMessage = "Tipe kerja wajib dipilih.")]
        public int? EmploymentTypeId { get; set; }

        [Required(ErrorMessage = "Status kerja wajib dipilih.")]
        public int? EmploymentStatusId { get; set; }

        public int? VendorId { get; set; }

        [Required(ErrorMessage = "Organisasi wajib dipilih.")]
        public int? OrganizationId { get; set; }

        public int? LocationId { get; set; }

        [Required(ErrorMessage = "Job level wajib dipilih.")]
        public int? JobLevelId { get; set; }

        [Required(ErrorMessage = "Jabatan wajib dipilih.")]
        public int? JobTitleId { get; set; }

        public int? GradeId { get; set; }
        public bool IsFte { get; set; } = true;
        public bool IsSales { get; set; }

        [DataType(DataType.Date)]
        public DateOnly? ContractEndDate { get; set; }

        public int? DirectManagerId { get; set; }
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
                EmployeeNumber = Input.EmployeeNumber,
                FullName = Input.FullName,
                WorkEmail = Input.WorkEmail,
                BirthDate = Input.BirthDate,
                JoinDate = Input.JoinDate,
                NationalityCountryId = Input.NationalityCountryId,
                ReligionId = Input.ReligionId,
                GenderId = Input.GenderId,
                MaritalStatusId = Input.MaritalStatusId,
                EmploymentTypeId = Input.EmploymentTypeId!.Value,
                EmploymentStatusId = Input.EmploymentStatusId!.Value,
                VendorId = Input.VendorId,
                OrganizationId = Input.OrganizationId!.Value,
                LocationId = Input.LocationId,
                JobLevelId = Input.JobLevelId!.Value,
                JobTitleId = Input.JobTitleId!.Value,
                GradeId = Input.GradeId,
                IsFte = Input.IsFte,
                IsSales = Input.IsSales,
                ContractEndDate = Input.ContractEndDate,
                DirectManagerId = Input.DirectManagerId
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

    private async Task<List<SelectListItem>> OptionsAsync(string type) =>
        ToItems(await _referenceService.GetOptionsAsync(type));

    private static List<SelectListItem> ToItems(List<ReferenceItemDto> items) =>
        items.Select(i => new SelectListItem(i.Name, i.Id.ToString())).ToList();

    private async Task LoadOptionsAsync()
    {
        GenderOptions = await OptionsAsync("gender");
        ReligionOptions = await OptionsAsync("religion");
        MaritalStatusOptions = await OptionsAsync("maritalstatus");
        CountryOptions = await OptionsAsync("country");
        EmploymentTypeOptions = await OptionsAsync("employmenttype");
        EmploymentStatusOptions = await OptionsAsync("employmentstatus");
        VendorOptions = await OptionsAsync("vendor");
        LocationOptions = await OptionsAsync("location");
        JobLevelOptions = await OptionsAsync("joblevel");
        JobTitleOptions = await OptionsAsync("jobtitle");
        GradeOptions = await OptionsAsync("grade");

        var organizations = await _organizationService.GetAllAsync();
        OrganizationOptions = organizations
            .Select(o => new SelectListItem(new string('\u2014', o.OrganizationLevel - 1) + " " + o.OrganizationName, o.Id.ToString()))
            .ToList();

        var employees = await _employeeService.GetAllAsync();
        ManagerOptions = employees
            .Select(e => new SelectListItem($"{e.FullName} ({e.EmployeeNumber})", e.Id.ToString()))
            .ToList();
    }
}
