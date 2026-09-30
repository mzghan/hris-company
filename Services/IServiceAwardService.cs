using HRIS.Api.Common; using HRIS.Api.DTOs.ServiceAward;
namespace HRIS.Api.Services; public interface IServiceAwardService { Task<List<ServiceAwardResponseDto>> GetAsync(UserContext actor); Task MarkStatusAsync(int id,string status,UserContext actor); }
