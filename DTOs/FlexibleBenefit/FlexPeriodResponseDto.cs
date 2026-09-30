namespace HRIS.Api.DTOs.FlexibleBenefit;
public class FlexPeriodResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public bool IsOpen { get; set; }
}
