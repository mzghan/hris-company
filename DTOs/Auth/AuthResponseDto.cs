namespace HRIS.Api.DTOs.Auth;

public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();

    // Bukan role tersimpan: true kalau karyawan punya bawahan aktif di Hierarchy.
    public bool IsManager { get; set; }
    public int? EmployeeId { get; set; }
    public DateTime ExpiresAt { get; set; }
}
