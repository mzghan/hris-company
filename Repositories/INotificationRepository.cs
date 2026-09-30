using HRIS.Api.Models;

namespace HRIS.Api.Repositories;

public interface INotificationRepository
{
    // Notifikasi in-app dan antrean email disimpan dalam SATU SaveChanges, jadi keduanya
    // berhasil atau gagal bersama.
    Task AddRangeAsync(IEnumerable<Notification> notifications, IEnumerable<EmailOutbox> emails);

    Task<List<Notification>> GetByUserAsync(int userId, bool unreadOnly, int take);
    Task<int> CountUnreadAsync(int userId);
    Task<Notification?> GetByIdAsync(int id);
    Task<List<Notification>> GetUnreadByUserAsync(int userId);
    Task UpdateAsync(Notification notification);
    Task UpdateRangeAsync(IEnumerable<Notification> notifications);
}
