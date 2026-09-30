using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.Models;

// REF_Organization. Menggantikan Department lama: bertingkat lewat ParentId,
// dan tingkatannya (divisi/departemen/dst) dibaca dari OrganizationLevel.
public class Organization : IAuditable
{
    public int Id { get; set; }

    [Required, MaxLength(150)]
    public string OrganizationName { get; set; } = string.Empty;

    public int? ParentId { get; set; }
    public Organization? Parent { get; set; }
    public ICollection<Organization> Children { get; set; } = new List<Organization>();

    // 1 = paling atas (mis. Company/Division), makin besar makin dalam.
    public int OrganizationLevel { get; set; } = 1;

    // Jalur dari root, mis. "/1/4/9/". Dipakai supaya query "semua karyawan di
    // satu divisi beserta anak departemennya" cukup pakai LIKE, tanpa recursive CTE.
    [MaxLength(500)]
    public string Path { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
}
