using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.Models;

// MST_Employee. Data pribadi karyawan. Data pekerjaan (jabatan, organisasi,
// grade, lokasi) ada di MST_Employee_Employment, dan atasan ada di
// MST_Employee_Hierarchy — bukan lagi kolom di tabel ini.
public class Employee : IAuditable
{
    public int Id { get; set; }

    // NIK karyawan: dipakai di surat, laporan, dan pencarian.
    [Required, MaxLength(30)]
    public string EmployeeNumber { get; set; } = string.Empty;

    [Required, MaxLength(150)]
    public string FullName { get; set; } = string.Empty;

    public DateOnly? BirthDate { get; set; }

    // Tanggal masuk pertama. Dasar hitung kelipatan 5 tahun (modul 5) dan due date
    // probation (modul 11). Sengaja bukan Employment.StartDate karena itu berubah
    // tiap kontrak/mutasi.
    public DateOnly JoinDate { get; set; }

    public int? NationalityCountryId { get; set; }
    public Country? NationalityCountry { get; set; }

    public int? ReligionId { get; set; }
    public Religion? Religion { get; set; }

    public int? GenderId { get; set; }
    public Gender? Gender { get; set; }

    public int? MaritalStatusId { get; set; }
    public MaritalStatus? MaritalStatus { get; set; }

    // Filter cepat saja; status resmi tetap dari Employment terkini.
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }

    public ICollection<EmployeeAddress> Addresses { get; set; } = new List<EmployeeAddress>();
    public ICollection<EmployeeContact> Contacts { get; set; } = new List<EmployeeContact>();
    public ICollection<EmployeeIdentity> Identities { get; set; } = new List<EmployeeIdentity>();
    public ICollection<EmployeeFinancialAccount> FinancialAccounts { get; set; } = new List<EmployeeFinancialAccount>();
    public ICollection<EmployeeFamily> Families { get; set; } = new List<EmployeeFamily>();
    public ICollection<EmployeeEmployment> Employments { get; set; } = new List<EmployeeEmployment>();
    public ICollection<EmployeeEducation> Educations { get; set; } = new List<EmployeeEducation>();
    public ICollection<EmployeeWorkExperience> WorkExperiences { get; set; } = new List<EmployeeWorkExperience>();

    // Atasan (baris di mana employee ini sebagai bawahan) dan bawahan (sebagai manager).
    public ICollection<EmployeeHierarchy> Managers { get; set; } = new List<EmployeeHierarchy>();
    public ICollection<EmployeeHierarchy> Subordinates { get; set; } = new List<EmployeeHierarchy>();
}
