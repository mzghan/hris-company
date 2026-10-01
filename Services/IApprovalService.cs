using HRIS.Api.Common;
using HRIS.Api.DTOs.Approval;

namespace HRIS.Api.Services;

public interface IApprovalService
{
    // Dipanggil modul saat pengajuan dibuat. Approver di-resolve dari REF_Approval_Flow_Step
    // dan disimpan sebagai snapshot. requestedDays dipakai langkah yang punya min_requested_days.
    Task<ApprovalRequestResponseDto> SubmitAsync(
        string requestType, int requestRefId, int requesterEmployeeId, string summary, int? requestedDays = null);
    Task<ApprovalRequestResponseDto> SubmitPersonalActionAsync(int requestRefId, int requesterEmployeeId, int? oldManagerId, int? newManagerId, string summary);

    Task<ApprovalRequestResponseDto> GetByIdAsync(int approvalId, UserContext actor);
    Task<Dictionary<int, ApprovalRequestResponseDto>> GetByRefsAsync(string requestType, IEnumerable<int> requestRefIds, UserContext? actor = null);

    // Inbox "menunggu saya". all = true (HR/Support) menampilkan seluruh pengajuan yang masih berjalan.
    Task<List<ApprovalRequestResponseDto>> GetInboxAsync(UserContext actor, bool all = false);
    Task<List<ApprovalRequestResponseDto>> GetMyRequestsAsync(UserContext actor);

    Task<ApprovalRequestResponseDto> ApproveAsync(int approvalId, UserContext actor, string? note);
    Task<ApprovalRequestResponseDto> RejectAsync(int approvalId, UserContext actor, string? note);
    Task<ApprovalRequestResponseDto> CancelAsync(int approvalId, UserContext actor);

    // Konfigurasi alur
    Task<List<ApprovalFlowStepResponseDto>> GetFlowStepsAsync();
    Task<ApprovalFlowStepResponseDto> UpdateFlowStepAsync(int id, ApprovalFlowStepUpdateDto dto);
}
