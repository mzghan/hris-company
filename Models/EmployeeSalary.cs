using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.Models;

// Riwayat gaji per employee. Sengaja dipisah dari Employee supaya kalau
// gaji naik, histori lama tetap ada (insert baris baru, bukan overwrite
// baris lama) — dipakai juga sebagai dasar perhitungan PayrollItem.
public class EmployeeSalary
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    [Range(0, double.MaxValue)]
    public decimal BaseSalary { get; set; }

    [Range(0, double.MaxValue)]
    public decimal AllowanceTotal { get; set; }

    // Tanggal mulai berlaku. "Gaji aktif" employee = baris dengan
    // EffectiveDate terbesar yang <= tanggal PayrollPeriod dibuat.
    public DateOnly EffectiveDate { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
