using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.Models;

// TRX_Letter_Request. Approval final state dicerminkan ke Status.
public class LetterRequest : IAuditable
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }
    public int LetterTypeId { get; set; }
    public LetterType? LetterType { get; set; }
    [Required, MaxLength(500)] public string Purpose { get; set; } = string.Empty;
    [MaxLength(300)] public string? Destination { get; set; }
    [Required, MaxLength(20)] public string Status { get; set; } = "Pending";
    public int? IssuedDocumentId { get; set; }
    public Document? IssuedDocument { get; set; }
    public DateTime? IssuedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
}
