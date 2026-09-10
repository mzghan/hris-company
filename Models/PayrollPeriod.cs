using HRIS.Api.Models.Enums;

namespace HRIS.Api.Models;

// Satu baris = satu periode gajian (mis. bulan 11 tahun 2026).
public class PayrollPeriod
{
    public int Id { get; set; }

    public int Month { get; set; }
    public int Year { get; set; }

    public PayrollPeriodStatus Status { get; set; } = PayrollPeriodStatus.Draft;

    // Sama seperti LeaveRequest.CurrentLevel: menunjuk level PayrollApproval
    // mana yang sedang aktif/ditunggu.
    public int CurrentLevel { get; set; } = 1;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<PayrollItem> Items { get; set; } = new List<PayrollItem>();
    public ICollection<PayrollApproval> Approvals { get; set; } = new List<PayrollApproval>();
}
