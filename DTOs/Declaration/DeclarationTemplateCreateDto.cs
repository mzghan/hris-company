using System.ComponentModel.DataAnnotations;
namespace HRIS.Api.DTOs.Declaration;
public class DeclarationTemplateCreateDto { [Required, MaxLength(200)] public string Title { get; set; } = string.Empty; [Required] public string Content { get; set; } = string.Empty; }
