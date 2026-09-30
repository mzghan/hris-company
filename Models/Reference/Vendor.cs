using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.Models;

// REF_Vendor. Wajib terisi di Employment kalau tipe kerjanya Outsource.
public class Vendor
{
    public int Id { get; set; }

    [Required, MaxLength(150)]
    public string VendorName { get; set; } = string.Empty;
}
