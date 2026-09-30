using System.ComponentModel.DataAnnotations;
namespace HRIS.Api.Models;
public class WorkType
{
    public int Id { get; set; }
    [Required, MaxLength(50)] public string Name { get; set; } = string.Empty;
}
