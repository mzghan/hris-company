using HRIS.Api.Models;
using HRIS.Api.Common;
namespace HRIS.Api.Services;
public interface ILeaveAdminService
{
    Task<List<LeaveBalance>> GetBalancesAsync(int year, UserContext actor);
    Task UpdateBalanceAsync(int id,int entitlement,int used,int sold,int carriedOver,UserContext actor);
    Task<List<PublicHoliday>> GetHolidaysAsync(int year,UserContext actor);
    Task AddHolidayAsync(DateOnly date,string name,UserContext actor);
    Task DeleteHolidayAsync(int id,UserContext actor);
}
