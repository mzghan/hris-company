using System.ComponentModel.DataAnnotations;
namespace HRIS.Api.Models;
public class JobDescriptionMaster : IAuditable
{
    public int Id { get; set; }
    [Required,MaxLength(100)] public string Code { get; set; }="";
    [Required,MaxLength(200)] public string JobTitle { get; set; }="";
    public string SnapshotJson { get; set; }="{}";
    public DateTime FinalizedAt {get;set;}=DateTime.UtcNow;
    public int? JobHolderEmployeeId {get;set;} public Employee? JobHolderEmployee{get;set;}
    public int? ImmediateManagerEmployeeId {get;set;} public Employee? ImmediateManagerEmployee{get;set;}
    public DateTime CreatedAt {get;set;}=DateTime.UtcNow; public int? CreatedBy{get;set;} public DateTime? UpdatedAt{get;set;} public int? UpdatedBy{get;set;}
}
