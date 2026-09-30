using System.ComponentModel.DataAnnotations;
namespace HRIS.Api.Models;
public class MeetingRoom
{
    public int Id { get; set; }
    [Required, MaxLength(120)] public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public int LocationId { get; set; }
    public Location? Location { get; set; }
    public bool IsActive { get; set; } = true;
}
