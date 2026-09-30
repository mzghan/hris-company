using HRIS.Api.Models;

namespace HRIS.Api.Repositories;

public interface IAssistanceRequestRepository
{
    Task<List<AssistanceRequest>> GetAllAsync(bool includeAnonymous);
    Task<List<AssistanceRequest>> GetByEmployeeAsync(int employeeId);
    Task<AssistanceRequest?> GetByIdAsync(int id);
    Task<AssistanceRequest> AddAsync(AssistanceRequest request);
    Task UpdateAsync(AssistanceRequest request);
}
