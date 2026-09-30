using System.ComponentModel.DataAnnotations;
namespace HRIS.Api.Models;
public class PublicHoliday
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    [Required, MaxLength(150)] public string Name { get; set; } = string.Empty;
}
