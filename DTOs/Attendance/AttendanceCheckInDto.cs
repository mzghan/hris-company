namespace HRIS.Api.DTOs.Attendance;

// EmployeeId diambil dari JWT claim/cookie yang sedang login, bukan dari
// body, supaya seorang employee tidak bisa absen atas nama orang lain.
public class AttendanceCheckInDto
{
    // Foto selfie saat check-in, dikirim sebagai base64 data URL
    // (mis. "data:image/jpeg;base64,...") hasil capture kamera di browser.
    public string? PhotoBase64 { get; set; }

    // Koordinat GPS dari Geolocation API browser saat check-in.
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}
