namespace HRIS.Api.DTOs.Declaration;
public class DeclarationSubmissionResponseDto { public int Id { get; set; } public int TemplateId { get; set; } public string TemplateTitle { get; set; } = string.Empty; public int EmployeeId { get; set; } public string EmployeeName { get; set; } = string.Empty; public DateTime AgreedAt { get; set; } public int? DocumentId { get; set; } }
