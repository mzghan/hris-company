using System.ComponentModel.DataAnnotations;
namespace HRIS.Api.DTOs.FlexibleBenefit;
public class LeaveEncashmentCreateDto
{
    [Required] public int PeriodId { get; set; }
    [Required] public int LeaveTypeId { get; set; }
    [Range(1,365)] public int Days { get; set; }
}
