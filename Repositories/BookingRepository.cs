using HRIS.Api.Data;
using HRIS.Api.Models;
using Microsoft.EntityFrameworkCore;
namespace HRIS.Api.Repositories;
public class BookingRepository : IBookingRepository
{
    private readonly AppDbContext _context;
    public BookingRepository(AppDbContext context) => _context = context;
    public Task<List<MeetingRoom>> GetRoomsAsync(bool includeInactive=false) =>
        _context.MeetingRooms.Include(x=>x.Location).Where(x=>includeInactive||x.IsActive).OrderBy(x=>x.Name).ToListAsync();
    public Task<MeetingRoom?> GetRoomAsync(int id) => _context.MeetingRooms.Include(x=>x.Location).FirstOrDefaultAsync(x=>x.Id==id);
    public async Task<MeetingRoom> AddRoomAsync(MeetingRoom room){_context.MeetingRooms.Add(room);await _context.SaveChangesAsync();return room;}
    public async Task SaveRoomAsync(MeetingRoom room)=>await _context.SaveChangesAsync();
    public Task<bool> HasConflictAsync(int roomId, DateTime start, DateTime end, int? excludeId=null) =>
        _context.RoomBookings.AnyAsync(x=>x.RoomId==roomId && x.Status!="Cancelled" &&
            (excludeId==null || x.Id!=excludeId.Value) && x.StartTime < end && x.EndTime > start);
    public Task<List<RoomBooking>> GetBookingsAsync(int? employeeId=null, DateTime? from=null, DateTime? to=null) =>
        _context.RoomBookings.Include(x=>x.Room).Include(x=>x.BookedByEmployee)
            .Where(x=>(employeeId==null||x.BookedByEmployeeId==employeeId) &&
                       (from==null||x.EndTime>=from) && (to==null||x.StartTime<=to))
            .OrderBy(x=>x.StartTime).ToListAsync();
    public Task<RoomBooking?> GetBookingAsync(int id) =>
        _context.RoomBookings.Include(x=>x.Room).Include(x=>x.BookedByEmployee).FirstOrDefaultAsync(x=>x.Id==id);
    public async Task<RoomBooking> AddAsync(RoomBooking booking){_context.RoomBookings.Add(booking);await _context.SaveChangesAsync();return booking;}
    public async Task UpdateAsync(RoomBooking booking)=>await _context.SaveChangesAsync();
}
