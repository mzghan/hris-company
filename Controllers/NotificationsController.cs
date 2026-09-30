using HRIS.Api.DTOs.Notification;
using HRIS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRIS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _service;

    public NotificationsController(INotificationService service)
    {
        _service = service;
    }

    [HttpGet("me")]
    public async Task<ActionResult<List<NotificationResponseDto>>> GetMine([FromQuery] bool unreadOnly = false, [FromQuery] int take = 50) =>
        Ok(await _service.GetMyAsync(User.GetUserId(), unreadOnly, take));

    [HttpGet("me/unread-count")]
    public async Task<ActionResult<int>> GetUnreadCount() =>
        Ok(await _service.GetUnreadCountAsync(User.GetUserId()));

    [HttpPost("{id:int}/read")]
    public async Task<IActionResult> MarkRead(int id)
    {
        await _service.MarkReadAsync(id, User.GetUserId());
        return NoContent();
    }

    [HttpPost("read-all")]
    public async Task<IActionResult> MarkAllRead()
    {
        await _service.MarkAllReadAsync(User.GetUserId());
        return NoContent();
    }
}
