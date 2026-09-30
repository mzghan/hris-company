using HRIS.Api.Controllers;
using HRIS.Api.DTOs.Notification;
using HRIS.Api.Exceptions;
using HRIS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HRIS.Api.Pages.Notifications;

[Authorize(AuthenticationSchemes = "Cookies")]
public class IndexModel : PageModel
{
    private readonly INotificationService _service;

    public IndexModel(INotificationService service)
    {
        _service = service;
    }

    public List<NotificationResponseDto> Items { get; set; } = new();

    public async Task OnGetAsync()
    {
        Items = await _service.GetMyAsync(User.GetUserId(), unreadOnly: false, take: 100);
    }

    // Klik notifikasi: tandai dibaca lalu buka halaman tujuannya.
    public async Task<IActionResult> OnGetOpenAsync(int id, string? to)
    {
        try
        {
            await _service.MarkReadAsync(id, User.GetUserId());
        }
        catch (NotFoundException)
        {
            return RedirectToPage();
        }

        // Hanya URL lokal, supaya tidak jadi open redirect.
        return !string.IsNullOrEmpty(to) && Url.IsLocalUrl(to) ? LocalRedirect(to) : RedirectToPage();
    }

    public async Task<IActionResult> OnPostReadAllAsync()
    {
        await _service.MarkAllReadAsync(User.GetUserId());
        return RedirectToPage();
    }
}
