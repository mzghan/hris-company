using HRIS.Api.Data;
using HRIS.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HRIS.Api.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly AppDbContext _context;

    public NotificationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddRangeAsync(IEnumerable<Notification> notifications, IEnumerable<EmailOutbox> emails)
    {
        _context.Notifications.AddRange(notifications);
        _context.EmailOutbox.AddRange(emails);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Notification>> GetByUserAsync(int userId, bool unreadOnly, int take) =>
        await _context.Notifications
            .Where(n => n.UserId == userId && (!unreadOnly || !n.IsRead))
            .OrderByDescending(n => n.CreatedAt).ThenByDescending(n => n.Id)
            .Take(take)
            .ToListAsync();

    public async Task<int> CountUnreadAsync(int userId) =>
        await _context.Notifications.CountAsync(n => n.UserId == userId && !n.IsRead);

    public async Task<Notification?> GetByIdAsync(int id) =>
        await _context.Notifications.FirstOrDefaultAsync(n => n.Id == id);

    public async Task<List<Notification>> GetUnreadByUserAsync(int userId) =>
        await _context.Notifications.Where(n => n.UserId == userId && !n.IsRead).ToListAsync();

    public async Task UpdateAsync(Notification notification)
    {
        _context.Notifications.Update(notification);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateRangeAsync(IEnumerable<Notification> notifications)
    {
        _context.Notifications.UpdateRange(notifications);
        await _context.SaveChangesAsync();
    }
}
