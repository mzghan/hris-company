using HRIS.Api.Common;
using HRIS.Api.DTOs.Notification;
using HRIS.Api.Exceptions;
using HRIS.Api.Models;
using HRIS.Api.Repositories;
using Microsoft.Extensions.Options;

namespace HRIS.Api.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _repository;
    private readonly IUserRepository _userRepository;
    private readonly EmailOptions _emailOptions;

    public NotificationService(
        INotificationRepository repository,
        IUserRepository userRepository,
        IOptions<EmailOptions> emailOptions)
    {
        _repository = repository;
        _userRepository = userRepository;
        _emailOptions = emailOptions.Value;
    }

    public async Task NotifyUsersAsync(IEnumerable<int> userIds, string title, string message, string? linkUrl = null)
    {
        var ids = userIds.Distinct().ToList();
        if (ids.Count == 0) return;

        var notifications = ids.Select(id => new Notification
        {
            UserId = id,
            Title = Truncate(title, 200),
            Message = Truncate(message, 1000),
            LinkUrl = linkUrl
        }).ToList();

        var emails = new List<EmailOutbox>();
        if (!string.Equals(_emailOptions.Mode, "Off", StringComparison.OrdinalIgnoreCase))
        {
            var contacts = await _userRepository.GetContactsAsync(ids);
            emails = contacts
                .Where(c => !string.IsNullOrWhiteSpace(c.Email))
                .Select(c => new EmailOutbox
                {
                    ToAddress = c.Email!,
                    ToName = c.Name,
                    Subject = Truncate($"[HRIS] {title}", 200),
                    Body = BuildBody(c.Name, message, linkUrl)
                })
                .ToList();
        }

        await _repository.AddRangeAsync(notifications, emails);
    }

    public async Task<List<NotificationResponseDto>> GetMyAsync(int userId, bool unreadOnly = false, int take = 50)
    {
        var items = await _repository.GetByUserAsync(userId, unreadOnly, Math.Clamp(take, 1, 200));
        return items.Select(ToDto).ToList();
    }

    public async Task<int> GetUnreadCountAsync(int userId) =>
        await _repository.CountUnreadAsync(userId);

    public async Task MarkReadAsync(int notificationId, int userId)
    {
        // Notifikasi milik user lain diperlakukan seperti tidak ada (tidak membocorkan keberadaannya).
        var notification = await _repository.GetByIdAsync(notificationId);
        if (notification is null || notification.UserId != userId)
            throw new NotFoundException("Notifikasi tidak ditemukan.");

        if (notification.IsRead) return;

        notification.IsRead = true;
        notification.ReadAt = DateTime.UtcNow;
        await _repository.UpdateAsync(notification);
    }

    public async Task MarkAllReadAsync(int userId)
    {
        var unread = await _repository.GetUnreadByUserAsync(userId);
        if (unread.Count == 0) return;

        var now = DateTime.UtcNow;
        foreach (var n in unread)
        {
            n.IsRead = true;
            n.ReadAt = now;
        }
        await _repository.UpdateRangeAsync(unread);
    }

    private string BuildBody(string name, string message, string? linkUrl)
    {
        var body = $"Halo {name},\n\n{message}\n";
        if (!string.IsNullOrWhiteSpace(linkUrl))
            body += $"\nBuka di HRIS: {_emailOptions.BaseUrl.TrimEnd('/')}{linkUrl}\n";
        body += "\nEmail ini dikirim otomatis, mohon tidak membalas.";
        return body;
    }

    private static string Truncate(string value, int max) =>
        value.Length <= max ? value : value[..max];

    private static NotificationResponseDto ToDto(Notification n) => new()
    {
        Id = n.Id,
        Title = n.Title,
        Message = n.Message,
        LinkUrl = n.LinkUrl,
        IsRead = n.IsRead,
        CreatedAt = n.CreatedAt
    };
}
