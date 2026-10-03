namespace HRIS.Api.Models;
public class JobDescriptionRevision : IAuditable
{
    public int Id { get; set; }
    public int JobDescriptionId { get; set; }
    public JobDescription? JobDescription { get; set; }
    public int RevisionNo { get; set; }
    public string SnapshotJson { get; set; } = "{}";
    public string? ChangeReason { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
}
