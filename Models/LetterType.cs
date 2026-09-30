using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.Models;

// REF_Letter_Type.
public class LetterType
{
    public int Id { get; set; }
    [Required, MaxLength(150)] public string Name { get; set; } = string.Empty;
    public int? TemplateDocumentId { get; set; }
    public Document? TemplateDocument { get; set; }
    public bool IsActive { get; set; } = true;
}
