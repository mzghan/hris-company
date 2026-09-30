namespace HRIS.Api.DTOs.FlexibleBenefit;
public class LeaveBalanceResponseDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public int LeaveTypeId { get; set; }
    public string LeaveTypeName { get; set; } = string.Empty;
    public int Year { get; set; }
    public int Entitlement { get; set; }
    public int Used { get; set; }
    public int Sold { get; set; }
    public int CarriedOver { get; set; }
    public int Available => Entitlement + CarriedOver - Used - Sold;
}
