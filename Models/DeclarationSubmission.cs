using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.Models;

// TRX_Declaration_Submission. Tidak memakai approval engine pada desain Batch C.
public class DeclarationSubmission : IAuditable
{
    public int Id { get; set; }
    public int TemplateId { get; set; }
    public DeclarationTemplate? Template { get; set; }
    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }
    public DateTime AgreedAt { get; set; } = DateTime.UtcNow;
    public int? DocumentId { get; set; }
    public Document? Document { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
}
