using HRIS.Api.Common;
using HRIS.Api.DTOs.Booking;
using HRIS.Api.Exceptions;
using HRIS.Api.Models;
using HRIS.Api.Repositories;
namespace HRIS.Api.Services;
public class BookingService : IBookingService
{
    private readonly IBookingRepository _repo;
    private readonly IEmployeeRepository _employees;
    private readonly IAuditLogService _audit;
    private readonly IReferenceRepository _refs;
    public BookingService(IBookingRepository repo,IEmployeeRepository employees,IAuditLogService audit,IReferenceRepository refs){_repo=repo;_employees=employees;_audit=audit;_refs=refs;}

    public async Task<List<RoomBookingResponseDto>> GetBookingsAsync(UserContext actor, DateTime? from=null, DateTime? to=null)
    {
        var rows=await _repo.GetBookingsAsync(actor.IsHrOrSupport?null:actor.EmployeeId,from,to);
        return rows.Select(ToDto).ToList();
    }
    public Task<List<MeetingRoom>> GetRoomsAsync(bool includeInactive=false)=>_repo.GetRoomsAsync(includeInactive);
    public async Task<RoomBookingResponseDto> CreateAsync(RoomBookingCreateDto dto,UserContext actor)
    {
        var emp=actor.EmployeeId??throw new ForbiddenException("Akun ini tidak terhubung ke Employee.");
        if(dto.EndTime<=dto.StartTime)throw new BadRequestException("Waktu selesai harus setelah waktu mulai.");
        if(dto.StartTime< DateTime.Now.AddMinutes(-5))throw new BadRequestException("Waktu booking tidak boleh di masa lalu.");
        var room=await _repo.GetRoomAsync(dto.RoomId)??throw new BadRequestException("Ruang meeting tidak ditemukan.");
        if(!room.IsActive)throw new BadRequestException("Ruang meeting tidak aktif.");
        if(await _repo.HasConflictAsync(dto.RoomId,dto.StartTime,dto.EndTime))throw new BadRequestException("Ruang meeting sudah dibooking pada rentang waktu tersebut.");
        var x=await _repo.AddAsync(new RoomBooking{RoomId=room.Id,BookedByEmployeeId=emp,Title=dto.Title.Trim(),StartTime=dto.StartTime,EndTime=dto.EndTime,Status="Confirmed",IsPriority=actor.IsHrOrSupport&&dto.IsPriority});
        return ToDto(await _repo.GetBookingAsync(x.Id)!);
    }
    public async Task CancelAsync(int id,UserContext actor)
    {
        var x=await _repo.GetBookingAsync(id)??throw new NotFoundException("Booking tidak ditemukan.");
        if(!actor.IsHrOrSupport && x.BookedByEmployeeId!=actor.EmployeeId)throw new ForbiddenException("Hanya pembuat booking yang boleh membatalkan.");
        if(x.Status=="Cancelled")return;
        x.Status="Cancelled";await _repo.UpdateAsync(x);
        if(actor.IsSupport)await _audit.LogAsync(actor.UserId,"Booking.Cancel",nameof(RoomBooking),id,"Support membatalkan booking.");
    }
    public async Task<MeetingRoom> CreateRoomAsync(string name,int capacity,int locationId,UserContext actor)
    {
        if(!actor.IsHrOrSupport)throw new ForbiddenException("Hanya HR/Support yang boleh mengelola ruang meeting.");
        if(capacity<1)throw new BadRequestException("Kapasitas minimal 1.");
        if(locationId <= 0 || !await _refs.ExistsAsync<HRIS.Api.Models.Location>(locationId)) throw new BadRequestException("Lokasi meeting tidak ditemukan.");
        var x=await _repo.AddRoomAsync(new MeetingRoom{Name=name.Trim(),Capacity=capacity,LocationId=locationId,IsActive=true});
        if(actor.IsSupport)await _audit.LogAsync(actor.UserId,"MeetingRoom.Create",nameof(MeetingRoom),x.Id,$"Ruang {x.Name} dibuat.");
        return x;
    }
    public async Task SetRoomActiveAsync(int id,bool active,UserContext actor)
    {
        if(!actor.IsHrOrSupport)throw new ForbiddenException("Hanya HR/Support yang boleh mengelola ruang meeting.");
        var x=await _repo.GetRoomAsync(id)??throw new NotFoundException("Ruang meeting tidak ditemukan.");
        x.IsActive=active;await _repo.SaveRoomAsync(x);
        if(actor.IsSupport)await _audit.LogAsync(actor.UserId,"MeetingRoom.Status",nameof(MeetingRoom),id,$"IsActive={active}.");
    }
    private static RoomBookingResponseDto ToDto(RoomBooking x)=>new(){Id=x.Id,RoomId=x.RoomId,RoomName=x.Room?.Name??"",BookedByEmployeeId=x.BookedByEmployeeId,BookedByName=x.BookedByEmployee?.FullName??"",Title=x.Title,StartTime=x.StartTime,EndTime=x.EndTime,Status=x.Status,IsPriority=x.IsPriority};
}
