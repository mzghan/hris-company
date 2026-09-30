using HRIS.Api.Common;
using HRIS.Api.DTOs.Booking;
namespace HRIS.Api.Services;
public interface IBookingService
{
    Task<List<RoomBookingResponseDto>> GetBookingsAsync(UserContext actor, DateTime? from=null, DateTime? to=null);
    Task<List<HRIS.Api.Models.MeetingRoom>> GetRoomsAsync(bool includeInactive=false);
    Task<RoomBookingResponseDto> CreateAsync(RoomBookingCreateDto dto, UserContext actor);
    Task CancelAsync(int id, UserContext actor);
    Task<HRIS.Api.Models.MeetingRoom> CreateRoomAsync(string name, int capacity, int locationId, UserContext actor);
    Task SetRoomActiveAsync(int id, bool active, UserContext actor);
}
