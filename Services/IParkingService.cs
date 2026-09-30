using HRIS.Api.Common; using HRIS.Api.DTOs.Parking;
namespace HRIS.Api.Services; public interface IParkingService { Task<List<VehicleTypeOption>> GetVehicleTypesAsync(); Task<List<ParkingResponseDto>> GetAsync(UserContext actor); Task<ParkingResponseDto> CreateAsync(ParkingCreateDto dto,UserContext actor); Task UpdateStatusAsync(int id,string status,UserContext actor); }
public record VehicleTypeOption(int Id,string Name);
