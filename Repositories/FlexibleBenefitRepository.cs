using HRIS.Api.Data;
using HRIS.Api.Models;
using Microsoft.EntityFrameworkCore;
namespace HRIS.Api.Repositories;
public class FlexibleBenefitRepository : IFlexibleBenefitRepository
{
    private readonly AppDbContext _context;
    public FlexibleBenefitRepository(AppDbContext context)=>_context=context;
    public Task<List<LeaveType>> GetLeaveTypesAsync()=>_context.LeaveTypes.OrderBy(x=>x.Name).ToListAsync();
    public Task<LeaveType?> GetLeaveTypeAsync(int id)=>_context.LeaveTypes.FirstOrDefaultAsync(x=>x.Id==id);
    public Task<List<PublicHoliday>> GetHolidaysAsync(DateOnly start, DateOnly end)=>_context.PublicHolidays.Where(x=>x.Date>=start&&x.Date<=end).ToListAsync();
    public Task<LeaveBalance?> GetBalanceAsync(int employeeId,int leaveTypeId,int year)=>_context.LeaveBalances.Include(x=>x.LeaveType).FirstOrDefaultAsync(x=>x.EmployeeId==employeeId&&x.LeaveTypeId==leaveTypeId&&x.Year==year);
    public Task<List<LeaveBalance>> GetBalancesAsync(int employeeId,int year)=>_context.LeaveBalances.Include(x=>x.LeaveType).Where(x=>x.EmployeeId==employeeId&&x.Year==year).OrderBy(x=>x.LeaveType!.Name).ToListAsync();
    public async Task<LeaveBalance> AddBalanceAsync(LeaveBalance balance){_context.LeaveBalances.Add(balance);await _context.SaveChangesAsync();return balance;}
    public async Task SaveAsync()=>await _context.SaveChangesAsync();
    public Task<List<FlexPeriod>> GetPeriodsAsync(bool openOnly=false)=>_context.FlexPeriods.Where(x=>!openOnly||x.IsOpen).OrderByDescending(x=>x.StartDate).ToListAsync();
    public Task<FlexPeriod?> GetPeriodAsync(int id)=>_context.FlexPeriods.FirstOrDefaultAsync(x=>x.Id==id);
    public Task<List<LeaveEncashment>> GetEncashmentsAsync(int? employeeId=null)=>_context.LeaveEncashments.Include(x=>x.Employee).Include(x=>x.Period).Include(x=>x.LeaveType).Where(x=>employeeId==null||x.EmployeeId==employeeId).OrderByDescending(x=>x.CreatedAt).ToListAsync();
    public Task<LeaveEncashment?> GetEncashmentAsync(int id)=>_context.LeaveEncashments.Include(x=>x.Employee).Include(x=>x.Period).Include(x=>x.LeaveType).FirstOrDefaultAsync(x=>x.Id==id);
    public async Task<LeaveEncashment> AddEncashmentAsync(LeaveEncashment x){_context.LeaveEncashments.Add(x);await _context.SaveChangesAsync();return x;}
    public Task<List<HealthClaim>> GetClaimsAsync(int? employeeId=null)=>_context.HealthClaims.Include(x=>x.Employee).Include(x=>x.Period).Where(x=>employeeId==null||x.EmployeeId==employeeId).OrderByDescending(x=>x.CreatedAt).ToListAsync();
    public Task<HealthClaim?> GetClaimAsync(int id)=>_context.HealthClaims.Include(x=>x.Employee).Include(x=>x.Period).FirstOrDefaultAsync(x=>x.Id==id);
    public async Task<HealthClaim> AddClaimAsync(HealthClaim x){_context.HealthClaims.Add(x);await _context.SaveChangesAsync();return x;}
}
