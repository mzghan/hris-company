using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.Models;

// SYS_Role. Hanya 3 role tersimpan: Employee (semua orang), HR, Support (akses penuh).
// Manager/Head/Group Head BUKAN role: mereka diturunkan dari rantai atasan
// (MST_Employee_Hierarchy) dan dibaca sebagai claim "IsManager" saat login.
public class Role
{
    public int Id { get; set; }

    [Required, MaxLength(50)]
    public string RoleName { get; set; } = string.Empty;
}
