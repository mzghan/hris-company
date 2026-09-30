using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.Models;

// MST_Employee_FinancialAccount. Data master saja (berguna untuk pencairan klaim nanti), belum dipakai modul mana pun.
public class EmployeeFinancialAccount : IAuditable
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    public int AccountTypeId { get; set; }
    public AccountType? AccountType { get; set; }

    [Required, MaxLength(50)]
    public string AccountNumber { get; set; } = string.Empty;

    [MaxLength(150)]
    public string? AccountName { get; set; }

    [MaxLength(100)]
    public string? ProviderName { get; set; }

    public bool IsPrimary { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
}
