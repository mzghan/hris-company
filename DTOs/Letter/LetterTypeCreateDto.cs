using System.ComponentModel.DataAnnotations;
namespace HRIS.Api.DTOs.Letter;
public class LetterTypeCreateDto { [Required, MaxLength(150)] public string Name { get; set; } = string.Empty; public int? TemplateDocumentId { get; set; } }
