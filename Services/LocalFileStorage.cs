using HRIS.Api.Common;
using Microsoft.Extensions.Options;

namespace HRIS.Api.Services;

public class LocalFileStorage : IFileStorage
{
    private readonly string _root;

    public LocalFileStorage(IOptions<DocumentOptions> options, IWebHostEnvironment environment)
    {
        var configured = options.Value.StoragePath;
        _root = Path.GetFullPath(Path.IsPathRooted(configured)
            ? configured
            : Path.Combine(environment.ContentRootPath, configured));
        Directory.CreateDirectory(_root);
    }

    public async Task<string> SaveAsync(Stream content, string extension, CancellationToken cancellationToken = default)
    {
        // Nama file di disk = GUID, jadi nama asli dari user tidak pernah dipakai sebagai path.
        var relative = Path.Combine(DateTime.UtcNow.ToString("yyyy"), DateTime.UtcNow.ToString("MM"), $"{Guid.NewGuid():N}{extension}");
        var full = Resolve(relative);
        Directory.CreateDirectory(Path.GetDirectoryName(full)!);

        await using var target = new FileStream(full, FileMode.CreateNew, FileAccess.Write, FileShare.None);
        await content.CopyToAsync(target, cancellationToken);

        return relative.Replace('\\', '/');
    }

    public Stream OpenRead(string storedPath)
    {
        var full = Resolve(storedPath);
        if (!File.Exists(full))
            throw new FileNotFoundException("File dokumen tidak ditemukan di penyimpanan.");

        return new FileStream(full, FileMode.Open, FileAccess.Read, FileShare.Read);
    }

    public void Delete(string storedPath)
    {
        var full = Resolve(storedPath);
        if (File.Exists(full))
            File.Delete(full);
    }

    // Menolak path yang keluar dari folder root (path traversal).
    private string Resolve(string relative)
    {
        var full = Path.GetFullPath(Path.Combine(_root, relative));
        var rootWithSeparator = _root.EndsWith(Path.DirectorySeparatorChar) ? _root : _root + Path.DirectorySeparatorChar;
        if (!full.StartsWith(rootWithSeparator, StringComparison.Ordinal))
            throw new InvalidOperationException("Path file tidak valid.");

        return full;
    }
}
