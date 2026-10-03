using System.ComponentModel.DataAnnotations;
namespace HRIS.Api.Models;
public class JobDescriptionBank : IAuditable
{
    public int Id { get; set; }
    [Required,MaxLength(100)] public string Code { get; set; }="";
    [Required,MaxLength(200)] public string JobTitle { get; set; }="";
    public string SnapshotJson { get; set; }="{}";
    [MaxLength(30)] public string Status { get; set; }="Template";
    public DateTime CreatedAt {get;set;}=DateTime.UtcNow; public int? CreatedBy{get;set;} public DateTime? UpdatedAt{get;set;} public int? UpdatedBy{get;set;}
}
