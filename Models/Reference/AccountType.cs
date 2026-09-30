using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.Models;

// REF_Account_Type. Contoh: bank account, e-wallet, credit card.
public class AccountType
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string AccountTypeName { get; set; } = string.Empty;
}
