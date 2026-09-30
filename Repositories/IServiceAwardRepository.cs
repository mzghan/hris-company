using HRIS.Api.Models;
namespace HRIS.Api.Repositories;
public interface IServiceAwardRepository { Task<List<ServiceAward>> GetAllAsync(); Task<ServiceAward?> GetByIdAsync(int id); Task<bool> ExistsAsync(int employeeId,int serviceYears); Task<ServiceAward> AddAsync(ServiceAward x); Task UpdateAsync(ServiceAward x); }
