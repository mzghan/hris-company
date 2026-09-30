namespace HRIS.Api.Models;

// MST_Employee_Hierarchy. Menggantikan Employee.ManagerId lama.
// Aturan: satu Direct Manager aktif (EndDate null) per karyawan; Dotted Manager boleh lebih dari satu.
// Riwayat atasan dijaga lewat StartDate/EndDate — perubahan atasan = tutup baris lama + buat baris baru.
public class EmployeeHierarchy : IAuditable
{
    public int Id { get; set; }

    // Bawahan
    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    // Atasan
    public int ManagerId { get; set; }
    public Employee? Manager { get; set; }

    public int HierarchyTypeId { get; set; }
    public HierarchyType? HierarchyType { get; set; }

    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
}
