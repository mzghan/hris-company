using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.DTOs.Employee;

public class EmployeeCreateDto
{
    // --- Data pribadi (MST_Employee) ---
    [Required, MaxLength(30)]
    public string EmployeeNumber { get; set; } = string.Empty;

    [Required, MaxLength(150)]
    public string FullName { get; set; } = string.Empty;

    // Disimpan sebagai MST_Employee_Contact bertipe "Work Email".
    [Required, EmailAddress, MaxLength(150)]
    public string WorkEmail { get; set; } = string.Empty;

    public DateOnly? BirthDate { get; set; }

    [Required]
    public DateOnly JoinDate { get; set; }

    public int? NationalityCountryId { get; set; }
    public int? ReligionId { get; set; }
    public int? GenderId { get; set; }
    public int? MaritalStatusId { get; set; }

    // --- Pekerjaan awal (MST_Employee_Employment), StartDate = JoinDate ---
    public int EmploymentTypeId { get; set; }
    public int EmploymentStatusId { get; set; }

    // Wajib kalau EmploymentType = Outsource, harus kosong kalau bukan.
    public int? VendorId { get; set; }

    public int OrganizationId { get; set; }
    public int? LocationId { get; set; }
    public int JobLevelId { get; set; }
    public int JobTitleId { get; set; }
    public int? GradeId { get; set; }
    public bool IsFte { get; set; } = true;
    public bool IsSales { get; set; }
    public DateOnly? ContractEndDate { get; set; }

    // Atasan langsung (MST_Employee_Hierarchy tipe Direct Manager). Null = tidak punya atasan.
    public int? DirectManagerId { get; set; }
}
