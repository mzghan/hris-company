using HRIS.Api.Data;
using HRIS.Api.Models;
using Microsoft.EntityFrameworkCore;
namespace HRIS.Api.Repositories;
public class LeaveAdminRepository : ILeaveAdminRepository
{
    private readonly AppDbContext _context;
    public LeaveAdminRepository(AppDbContext context)=>_context=context;
    public Task<List<LeaveBalance>> GetAllBalancesAsync(int year)=>_context.LeaveBalances.Include(x=>x.Employee).Include(x=>x.LeaveType).Where(x=>x.Year==year).OrderBy(x=>x.Employee!.EmployeeNumber).ThenBy(x=>x.LeaveType!.Name).ToListAsync();
    public Task<LeaveBalance?> GetBalanceAsync(int id)=>_context.LeaveBalances.FirstOrDefaultAsync(x=>x.Id==id);
    public Task SaveAsync()=>_context.SaveChangesAsync();
    public Task<List<PublicHoliday>> GetHolidaysAsync(int year)=>_context.PublicHolidays.Where(x=>x.Date.Year==year).OrderBy(x=>x.Date).ToListAsync();
    public async Task<PublicHoliday> AddHolidayAsync(PublicHoliday holiday){_context.PublicHolidays.Add(holiday);await _context.SaveChangesAsync();return holiday;}
    public Task<PublicHoliday?> GetHolidayAsync(int id)=>_context.PublicHolidays.FirstOrDefaultAsync(x=>x.Id==id);
    public async Task DeleteHolidayAsync(PublicHoliday holiday){_context.PublicHolidays.Remove(holiday);await _context.SaveChangesAsync();}
}
