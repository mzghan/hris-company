using HRIS.Api.Models;
namespace HRIS.Api.Repositories;
public interface IBookingRepository
{
    Task<List<MeetingRoom>> GetRoomsAsync(bool includeInactive = false);
    Task<MeetingRoom?> GetRoomAsync(int id);
    Task<MeetingRoom> AddRoomAsync(MeetingRoom room);
    Task SaveRoomAsync(MeetingRoom room);
    Task<bool> HasConflictAsync(int roomId, DateTime start, DateTime end, int? excludeId = null);
    Task<List<RoomBooking>> GetBookingsAsync(int? employeeId = null, DateTime? from = null, DateTime? to = null);
    Task<RoomBooking?> GetBookingAsync(int id);
    Task<RoomBooking> AddAsync(RoomBooking booking);
    Task UpdateAsync(RoomBooking booking);
}
