namespace HRIS.Api.Common;

// Identitas user yang sedang bertindak (dibaca dari claim JWT/cookie). Dipakai Service
// supaya tidak bergantung ke ClaimsPrincipal (mudah dites, dan bisa dibuat manual di seeder).
public record UserContext(int UserId, int? EmployeeId, IReadOnlyCollection<string> Roles)
{
    public bool IsSupport => Roles.Contains(RoleNames.Support);
    public bool IsHr => Roles.Contains(RoleNames.HR);
    public bool IsHrOrSupport => IsSupport || IsHr;
    public bool IsHRBP => Roles.Contains(RoleNames.HRBP);
    public bool IsOE => Roles.Contains(RoleNames.OE);
    public bool IsManagerRole => Roles.Contains(RoleNames.Manager);
    public bool IsHead => Roles.Contains(RoleNames.Head);
    public bool IsAdmin => Roles.Contains(RoleNames.Admin);
    public bool IsTA => Roles.Contains(RoleNames.TA);
    public bool IsIntern => Roles.Contains(RoleNames.INTERN);

    // Referensi HRManpower: HRBP/OE/Admin dapat melihat seluruh data modul.
    public bool IsManpowerAdmin => IsHrOrSupport || IsHRBP || IsOE || IsAdmin;
    public bool IsManpowerApprover => IsManpowerAdmin || IsManagerRole || IsHead;

}
