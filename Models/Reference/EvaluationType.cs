using System.ComponentModel.DataAnnotations;
namespace HRIS.Api.Models;
public class EvaluationType
{
    public int Id { get; set; }
    [Required, MaxLength(100)] public string Name { get; set; } = string.Empty;
    public int? MonthOffset { get; set; }
}