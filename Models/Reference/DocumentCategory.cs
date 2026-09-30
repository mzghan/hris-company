using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.Models;

// REF_Document_Category. Kategori bertingkat lewat ParentId (mis. Forms > Medical).
public class DocumentCategory
{
    public int Id { get; set; }

    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    public int? ParentId { get; set; }
    public DocumentCategory? Parent { get; set; }
    public ICollection<DocumentCategory> Children { get; set; } = new List<DocumentCategory>();

    public bool IsActive { get; set; } = true;
}
