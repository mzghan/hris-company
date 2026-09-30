namespace HRIS.Api.Common;

public static class RoleNames
{
    public const string Employee = "Employee";
    public const string HR = "HR";
    public const string Support = "Support";

    // Dipakai di [Authorize(Roles = ...)] untuk aksi master data (HR + Support).
    public const string HrOrSupport = "HR,Support";

    // Nama policy untuk "atasan": bukan role, tapi claim IsManager dari Hierarchy.
    public const string ManagerOrHrPolicy = "ManagerOrHR";
    public const string IsManagerClaim = "IsManager";
}
