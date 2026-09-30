using HRIS.Api.Models;

namespace HRIS.Api.Repositories;

public interface IApprovalRepository
{
    Task<ApprovalRequest?> GetByIdAsync(int id);
    Task<ApprovalRequest?> GetByRefAsync(string requestType, int requestRefId);
    Task<List<ApprovalRequest>> GetByRefsAsync(string requestType, IEnumerable<int> requestRefIds);
    Task<List<ApprovalRequest>> GetByRequesterAsync(int requesterEmployeeId);

    // Pengajuan Pending yang langkah aktifnya ditugaskan ke employee ini (tipe Employee)
    // atau ke salah satu role-nya (tipe Role). Pengajuan milik sendiri tidak ikut.
    Task<List<ApprovalRequest>> GetInboxAsync(int? employeeId, IEnumerable<string> roleNames);
    Task<List<ApprovalRequest>> GetAllPendingAsync();

    Task<ApprovalRequest> AddAsync(ApprovalRequest request);
    Task UpdateAsync(ApprovalRequest request);

    // Konfigurasi alur (REF_Approval_Flow_Step)
    Task<List<ApprovalFlowStep>> GetActiveFlowStepsAsync(string requestType);
    Task<List<ApprovalFlowStep>> GetAllFlowStepsAsync();
    Task<ApprovalFlowStep?> GetFlowStepByIdAsync(int id);
    Task UpdateFlowStepAsync(ApprovalFlowStep step);
}
