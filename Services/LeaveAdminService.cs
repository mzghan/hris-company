using HRIS.Api.Common;
using HRIS.Api.Exceptions;
using HRIS.Api.Models;
using HRIS.Api.Repositories;
namespace HRIS.Api.Services;
public class LeaveAdminService : ILeaveAdminService
{
    private readonly ILeaveAdminRepository _repo;
    private readonly IAuditLogService _audit;
    public LeaveAdminService(ILeaveAdminRepository repo,IAuditLogService audit){_repo=repo;_audit=audit;}
    private static void Ensure(UserContext actor){if(!actor.IsHrOrSupport)throw new ForbiddenException("Hanya HR/Support yang boleh mengelola saldo cuti dan hari libur.");}
    public async Task<List<LeaveBalance>> GetBalancesAsync(int year,UserContext actor){Ensure(actor);return await _repo.GetAllBalancesAsync(year);}
    public async Task UpdateBalanceAsync(int id,int entitlement,int used,int sold,int carriedOver,UserContext actor)
    {
        Ensure(actor);
        if(entitlement<0||used<0||sold<0||carriedOver<0)throw new BadRequestException("Nilai saldo tidak boleh negatif.");
        var x=await _repo.GetBalanceAsync(id)??throw new NotFoundException("Saldo cuti tidak ditemukan.");
        x.Entitlement=entitlement;x.Used=used;x.Sold=sold;x.CarriedOver=carriedOver;await _repo.SaveAsync();
        if(actor.IsSupport)await _audit.LogAsync(actor.UserId,"LeaveBalance.Update",nameof(LeaveBalance),id,"Support mengubah saldo cuti.");
    }
    public async Task<List<PublicHoliday>> GetHolidaysAsync(int year,UserContext actor){Ensure(actor);return await _repo.GetHolidaysAsync(year);}
    public async Task AddHolidayAsync(DateOnly date,string name,UserContext actor)
    {
        Ensure(actor); if(string.IsNullOrWhiteSpace(name))throw new BadRequestException("Nama hari libur wajib diisi.");
        if((await _repo.GetHolidaysAsync(date.Year)).Any(x=>x.Date==date))throw new BadRequestException("Tanggal hari libur sudah ada.");
        var x=await _repo.AddHolidayAsync(new PublicHoliday{Date=date,Name=name.Trim()});
        if(actor.IsSupport)await _audit.LogAsync(actor.UserId,"PublicHoliday.Create",nameof(PublicHoliday),x.Id,$"Hari libur {x.Name} ditambahkan.");
    }
    public async Task DeleteHolidayAsync(int id,UserContext actor){Ensure(actor);var x=await _repo.GetHolidayAsync(id)??throw new NotFoundException("Hari libur tidak ditemukan.");await _repo.DeleteHolidayAsync(x);if(actor.IsSupport)await _audit.LogAsync(actor.UserId,"PublicHoliday.Delete",nameof(PublicHoliday),id,$"Hari libur {x.Name} dihapus.");}
}
