using HRIS.Api.Common; using HRIS.Api.DTOs.Laptop;
namespace HRIS.Api.Services; public interface ILaptopService { Task<List<LaptopResponseDto>> GetAsync(UserContext actor); Task<LaptopResponseDto> CreateAsync(LaptopCreateDto dto,UserContext actor); Task UpdateStatusAsync(int id,string status,string? note,UserContext actor); }
