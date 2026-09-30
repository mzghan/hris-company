namespace HRIS.Api.Common;

// Konfigurasi section "Documents" di appsettings.json.
public class DocumentOptions
{
    // Relatif terhadap ContentRoot, atau path absolut. Sengaja BUKAN di wwwroot,
    // supaya dokumen pribadi tidak bisa diakses langsung lewat URL statis.
    public string StoragePath { get; set; } = "App_Data/documents";

    public int MaxSizeMb { get; set; } = 10;

    // Kosong = pakai daftar bawaan di DocumentService.
    public string[] AllowedExtensions { get; set; } = Array.Empty<string>();
}
