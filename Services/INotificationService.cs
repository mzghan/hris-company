using HRIS.Api.DTOs.Notification;

namespace HRIS.Api.Services;

public interface INotificationService
{
    // Membuat notifikasi in-app untuk tiap user, dan (kalau Email:Mode bukan Off) mengantrekan
    // email ke Work Email mereka. Pengiriman email dilakukan background job, bukan di sini.
    Task NotifyUsersAsync(IEnumerable<int> userIds, string title, string message, string? linkUrl = null);

    Task<List<NotificationResponseDto>> GetMyAsync(int userId, bool unreadOnly = false, int take = 50);
    Task<int> GetUnreadCountAsync(int userId);
    Task MarkReadAsync(int notificationId, int userId);
    Task MarkAllReadAsync(int userId);
}
