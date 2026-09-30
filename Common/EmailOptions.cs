namespace HRIS.Api.Common;

// Konfigurasi section "Email" di appsettings.json.
public class EmailOptions
{
    // Off  = notifikasi hanya in-app, tidak ada email yang diantrekan.
    // Log  = email diantrekan dan "dikirim" ke log saja (default untuk development).
    // Smtp = email dikirim lewat SMTP sungguhan (isi Host/Port/Username/Password).
    public string Mode { get; set; } = "Log";

    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 587;
    public bool UseSsl { get; set; } = true;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public string FromAddress { get; set; } = "no-reply@hris.local";
    public string FromName { get; set; } = "HRIS";

    // Seberapa sering background job memeriksa antrean email.
    public int PollSeconds { get; set; } = 30;

    // Setelah gagal sebanyak ini, email ditandai Failed permanen.
    public int MaxAttempts { get; set; } = 5;

    // Dipakai membentuk link absolut di isi email.
    public string BaseUrl { get; set; } = "http://localhost:5000";
}
