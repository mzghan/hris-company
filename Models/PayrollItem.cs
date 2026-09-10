namespace HRIS.Api.Models;

// Hasil hitung gaji satu employee untuk satu PayrollPeriod. Digenerate
// otomatis dari EmployeeSalary aktif employee tsb saat PayrollPeriod dibuat.
public class PayrollItem
{
    public int Id { get; set; }

    public int PayrollPeriodId { get; set; }
    public PayrollPeriod? PayrollPeriod { get; set; }

    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    public decimal BaseSalary { get; set; }
    public decimal TotalAllowance { get; set; }

    // Rumus potongan (pajak, BPJS, dsb.) belum difinalkan di fase ini
    // (lihat bab 9.4 technical alignment doc) — default 0 untuk MVP.
    public decimal TotalDeduction { get; set; }

    public decimal GrossPay { get; set; }
    public decimal NetPay { get; set; }
}
