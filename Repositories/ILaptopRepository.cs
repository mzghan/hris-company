using HRIS.Api.Models;
namespace HRIS.Api.Repositories;
public interface ILaptopRepository { Task<List<LaptopRequest>> GetAllAsync(int? employeeId=null); Task<LaptopRequest?> GetByIdAsync(int id); Task<LaptopRequest> AddAsync(LaptopRequest x); Task UpdateAsync(LaptopRequest x); Task<LaptopStatusLog> AddLogAsync(LaptopStatusLog x); }
