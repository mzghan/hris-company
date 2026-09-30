namespace HRIS.Api.Common;

// Identitas user yang sedang bertindak (dibaca dari claim JWT/cookie). Dipakai Service
// supaya tidak bergantung ke ClaimsPrincipal (mudah dites, dan bisa dibuat manual di seeder).
public record UserContext(int UserId, int? EmployeeId, IReadOnlyCollection<string> Roles)
{
    public bool IsSupport => Roles.Contains(RoleNames.Support);
    public bool IsHr => Roles.Contains(RoleNames.HR);
    public bool IsHrOrSupport => IsSupport || IsHr;
}
