using HRIS.Api.Models;
namespace HRIS.Api.Repositories;
public interface IFlexibleBenefitRepository
{
    Task<List<LeaveType>> GetLeaveTypesAsync();
    Task<LeaveType?> GetLeaveTypeAsync(int id);
    Task<List<PublicHoliday>> GetHolidaysAsync(DateOnly start, DateOnly end);
    Task<LeaveBalance?> GetBalanceAsync(int employeeId, int leaveTypeId, int year);
    Task<List<LeaveBalance>> GetBalancesAsync(int employeeId, int year);
    Task<LeaveBalance> AddBalanceAsync(LeaveBalance balance);
    Task SaveAsync();
    Task<List<FlexPeriod>> GetPeriodsAsync(bool openOnly=false);
    Task<FlexPeriod?> GetPeriodAsync(int id);
    Task<List<LeaveEncashment>> GetEncashmentsAsync(int? employeeId=null);
    Task<LeaveEncashment?> GetEncashmentAsync(int id);
    Task<LeaveEncashment> AddEncashmentAsync(LeaveEncashment x);
    Task<List<HealthClaim>> GetClaimsAsync(int? employeeId=null);
    Task<HealthClaim?> GetClaimAsync(int id);
    Task<HealthClaim> AddClaimAsync(HealthClaim x);
}
