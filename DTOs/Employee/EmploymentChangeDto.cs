namespace HRIS.Api.DTOs.Employee;

// Perubahan pekerjaan (mutasi, kenaikan grade, kontrak baru, dst): baris Employment
// lama ditutup (EndDate = EffectiveDate - 1 hari), lalu dibuat baris baru mulai EffectiveDate.
public class EmploymentChangeDto
{
    public DateOnly EffectiveDate { get; set; }

    public int EmploymentTypeId { get; set; }
    public int EmploymentStatusId { get; set; }
    public int? VendorId { get; set; }
    public int OrganizationId { get; set; }
    public int? LocationId { get; set; }
    public int JobLevelId { get; set; }
    public int JobTitleId { get; set; }
    public int? GradeId { get; set; }
    public bool IsFte { get; set; } = true;
    public bool IsSales { get; set; }
    public DateOnly? ContractEndDate { get; set; }

    // Alasan berakhirnya baris lama (opsional, mis. resign atau kontrak habis).
    public int? EndReasonId { get; set; }
}
