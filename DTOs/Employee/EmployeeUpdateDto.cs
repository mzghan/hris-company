using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.DTOs.Employee;

// Hanya data pribadi. Perubahan jabatan/organisasi/grade/lokasi lewat
// EmploymentChangeDto, dan perubahan atasan lewat ChangeManagerDto,
// supaya riwayatnya tersimpan.
public class EmployeeUpdateDto
{
    [Required, MaxLength(150)]
    public string FullName { get; set; } = string.Empty;

    public DateOnly? BirthDate { get; set; }
    public DateOnly JoinDate { get; set; }
    public int? NationalityCountryId { get; set; }
    public int? ReligionId { get; set; }
    public int? GenderId { get; set; }
    public int? MaritalStatusId { get; set; }
    public bool IsActive { get; set; } = true;
}
