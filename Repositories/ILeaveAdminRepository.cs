using HRIS.Api.Models;
namespace HRIS.Api.Repositories;
public interface ILeaveAdminRepository
{
    Task<List<LeaveBalance>> GetAllBalancesAsync(int year);
    Task<LeaveBalance?> GetBalanceAsync(int id);
    Task SaveAsync();
    Task<List<PublicHoliday>> GetHolidaysAsync(int year);
    Task<PublicHoliday> AddHolidayAsync(PublicHoliday holiday);
    Task<PublicHoliday?> GetHolidayAsync(int id);
    Task DeleteHolidayAsync(PublicHoliday holiday);
}
