using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.Models;

// MST_Employee_Employment. Baris terkini = EndDate null (satu per karyawan).
// Perubahan jabatan/grade/organisasi/lokasi = tutup baris lama (isi EndDate) + insert baris baru,
// jangan update di tempat supaya riwayatnya terjaga.
public class EmployeeEmployment : IAuditable
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    public int EmploymentStatusId { get; set; }
    public EmploymentStatus? EmploymentStatus { get; set; }

    public bool IsFte { get; set; } = true;
    public bool IsSales { get; set; }

    public int EmploymentTypeId { get; set; }
    public EmploymentType? EmploymentType { get; set; }

    // Wajib terisi kalau EmploymentType = Outsource, harus kosong untuk tipe lain (divalidasi di Service).
    public int? VendorId { get; set; }
    public Vendor? Vendor { get; set; }

    public int OrganizationId { get; set; }
    public Organization? Organization { get; set; }

    public int? LocationId { get; set; }
    public Location? Location { get; set; }

    public int JobLevelId { get; set; }
    public JobLevel? JobLevel { get; set; }

    public int JobTitleId { get; set; }
    public JobTitle? JobTitle { get; set; }

    public int? GradeId { get; set; }
    public Grade? Grade { get; set; }

    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public DateOnly? ContractEndDate { get; set; }

    public int? EndReasonId { get; set; }
    public EndReason? EndReason { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
}
