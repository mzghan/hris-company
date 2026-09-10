using System;

namespace HRIS.Api.Common;

// Helper kecil supaya semua timestamp absensi (dan tempat lain yang butuh
// "jam sekarang") konsisten pakai waktu Jakarta (WIB, UTC+7), bukan jam
// server/sistem yang belum tentu di-set ke timezone Indonesia.
public static class JakartaTime
{
    private static readonly TimeZoneInfo _zone = ResolveTimeZone();

    private static TimeZoneInfo ResolveTimeZone()
    {
        // "Asia/Jakarta" = ID IANA (Linux/macOS, dan Windows modern dgn ICU).
        // "SE Asia Standard Time" = ID lama khusus Windows.
        // Kalau dua-duanya tidak ada di OS, fallback ke offset manual +07:00
        // supaya fitur ini tidak pernah crash gara-gara timezone database.
        try { return TimeZoneInfo.FindSystemTimeZoneById("Asia/Jakarta"); }
        catch (TimeZoneNotFoundException) { }
        catch (InvalidTimeZoneException) { }

        try { return TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"); }
        catch (TimeZoneNotFoundException) { }
        catch (InvalidTimeZoneException) { }

        return TimeZoneInfo.CreateCustomTimeZone("WIB", TimeSpan.FromHours(7), "Waktu Indonesia Barat", "WIB");
    }

    /// <summary>Jam sekarang di Jakarta (WIB), dihitung dari UTC — bukan dari jam lokal server.</summary>
    public static DateTime Now() => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _zone);

    /// <summary>Tanggal hari ini di Jakarta (WIB).</summary>
    public static DateOnly Today() => DateOnly.FromDateTime(Now());
}
