using System.ComponentModel.DataAnnotations;
namespace HRIS.Api.Models;
public class Competency
{
    public int Id { get; set; }
    [Required, MaxLength(150)] public string Name { get; set; } = string.Empty;
    [MaxLength(1000)] public string? Description { get; set; }
}