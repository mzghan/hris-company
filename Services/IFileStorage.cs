namespace HRIS.Api.Services;

// Abstraksi penyimpanan file supaya nanti mudah dipindah (mis. ke object storage)
// tanpa menyentuh DocumentService.
public interface IFileStorage
{
    // Mengembalikan path relatif yang dibuat sistem (bukan dari input user).
    Task<string> SaveAsync(Stream content, string extension, CancellationToken cancellationToken = default);
    Stream OpenRead(string storedPath);
    void Delete(string storedPath);
}
