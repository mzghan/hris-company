using System.ComponentModel.DataAnnotations;
namespace HRIS.Api.Models;
public class LeaveType
{
    public int Id { get; set; }
    [Required, MaxLength(80)] public string Name { get; set; } = string.Empty;
    public bool IsSellable { get; set; }
}
