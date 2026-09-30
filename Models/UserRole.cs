namespace HRIS.Api.Models;

// SYS_User_Role (join table User <-> Role). Key-nya gabungan (UserId, RoleId).
public class UserRole
{
    public int UserId { get; set; }
    public User? User { get; set; }

    public int RoleId { get; set; }
    public Role? Role { get; set; }
}
