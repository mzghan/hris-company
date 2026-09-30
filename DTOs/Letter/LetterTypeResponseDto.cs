namespace HRIS.Api.DTOs.Letter;
public class LetterTypeResponseDto { public int Id { get; set; } public string Name { get; set; } = string.Empty; public int? TemplateDocumentId { get; set; } public bool IsActive { get; set; } }
