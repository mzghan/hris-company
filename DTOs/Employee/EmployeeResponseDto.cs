namespace HRIS.Api.DTOs.Employee;

public class EmployeeResponseDto
{
    public int Id { get; set; }
    public string EmployeeNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? WorkEmail { get; set; }
    public DateOnly? BirthDate { get; set; }
    public DateOnly JoinDate { get; set; }
    public bool IsActive { get; set; }

    public int? NationalityCountryId { get; set; }
    public string? NationalityCountryName { get; set; }
    public int? ReligionId { get; set; }
    public int? GenderId { get; set; }
    public string? GenderName { get; set; }
    public int? MaritalStatusId { get; set; }

    // --- Pekerjaan terkini (baris Employment dengan EndDate null) ---
    public int? EmploymentId { get; set; }
    public int? EmploymentTypeId { get; set; }
    public string? EmploymentTypeName { get; set; }
    public int? EmploymentStatusId { get; set; }
    public string? EmploymentStatusName { get; set; }
    public int? VendorId { get; set; }
    public string? VendorName { get; set; }
    public int? OrganizationId { get; set; }
    public string? OrganizationName { get; set; }
    public int? LocationId { get; set; }
    public string? LocationName { get; set; }
    public int? JobLevelId { get; set; }
    public string? JobLevelName { get; set; }
    public int? JobTitleId { get; set; }
    public string? JobTitleName { get; set; }
    public int? GradeId { get; set; }
    public int? GradeLevel { get; set; }
    public bool? IsFte { get; set; }
    public bool? IsSales { get; set; }
    public DateOnly? EmploymentStartDate { get; set; }
    public DateOnly? ContractEndDate { get; set; }

    // --- Atasan langsung aktif ---
    public int? DirectManagerId { get; set; }
    public string? DirectManagerName { get; set; }
}
