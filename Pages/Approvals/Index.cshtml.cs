using HRIS.Api.Controllers;
using HRIS.Api.DTOs.Approval;
using HRIS.Api.Exceptions;
using HRIS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HRIS.Api.Pages.Approvals;

// Data untuk partial _RequestTable.
public record ApprovalTableViewModel(List<ApprovalRequestResponseDto> Items, bool ShowActions, bool ShowCancel, string EmptyText);

// Inbox generik: semua jenis pengajuan (cuti, dan modul berikutnya) lewat approval engine.
[Authorize(AuthenticationSchemes = "Cookies")]
public class IndexModel : PageModel
{
    private readonly IApprovalService _service;

    public IndexModel(IApprovalService service)
    {
        _service = service;
    }

    public List<ApprovalRequestResponseDto> Inbox { get; set; } = new();
    public List<ApprovalRequestResponseDto> MyRequests { get; set; } = new();
    public List<ApprovalRequestResponseDto> AllPending { get; set; } = new();
    public bool IsHrOrSupport { get; set; }

    public ApprovalTableViewModel InboxTable => new(Inbox, true, false, "Tidak ada pengajuan yang menunggu persetujuan kamu.");
    public ApprovalTableViewModel MyTable => new(MyRequests, false, true, "Belum ada pengajuan.");
    public ApprovalTableViewModel AllTable => new(AllPending, true, false, "Tidak ada pengajuan yang sedang berjalan.");

    [BindProperty(SupportsGet = true)] public string? View { get; set; }

    public async Task OnGetAsync()
    {
        var actor = User.ToUserContext();
        IsHrOrSupport = actor.IsHrOrSupport;

        Inbox = await _service.GetInboxAsync(actor);
        MyRequests = await _service.GetMyRequestsAsync(actor);
        if (IsHrOrSupport)
            AllPending = await _service.GetInboxAsync(actor, all: true);
    }

    public async Task<IActionResult> OnPostApproveAsync(int id, string? note)
    {
        try
        {
            await _service.ApproveAsync(id, User.ToUserContext(), note);
            TempData["Success"] = "Keputusan tersimpan: disetujui.";
        }
        catch (Exception ex) when (ex is ForbiddenException or BadRequestException or NotFoundException)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRejectAsync(int id, string? note)
    {
        try
        {
            await _service.RejectAsync(id, User.ToUserContext(), note);
            TempData["Success"] = "Keputusan tersimpan: ditolak.";
        }
        catch (Exception ex) when (ex is ForbiddenException or BadRequestException or NotFoundException)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostCancelAsync(int id)
    {
        try
        {
            await _service.CancelAsync(id, User.ToUserContext());
            TempData["Success"] = "Pengajuan dibatalkan.";
        }
        catch (Exception ex) when (ex is ForbiddenException or BadRequestException or NotFoundException)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToPage();
    }
}
