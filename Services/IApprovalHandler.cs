using HRIS.Api.Models;
using HRIS.Api.Models.Enums;

namespace HRIS.Api.Services;

// Setiap modul yang memakai approval engine mendaftarkan satu handler untuk request_type-nya.
// OnCompletedAsync dipanggil di transaction yang sama dengan langkah approval terakhir
// (Approved / Rejected / Cancelled), jadi efek ke data modul (mengubah status, menerapkan
// perubahan master, menutup Employment lama, dst) atomik dengan keputusan approval-nya.
public interface IApprovalHandler
{
    string RequestType { get; }
    Task OnCompletedAsync(ApprovalRequest request, ApprovalRequestStatus finalStatus);
}
