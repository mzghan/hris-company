namespace HRIS.Api.Common;

public static class RoleNames
{
    public const string Employee = "Employee";
    public const string HR = "HR";
    public const string Support = "Support";

    // Role HR-Manpower/JD sesuai referensi modul HRManpower.
    public const string HRBP = "HRBP";
    public const string OE = "OE";
    public const string Manager = "Manager";
    public const string Head = "Head";
    public const string Admin = "Admin";
    public const string TA = "TA";
    public const string INTERN = "INTERN";

    public static readonly string[] HrManpowerRoles =
    {
        HRBP, OE, Manager, Head, Admin, TA, INTERN
    };

    // Dipakai di [Authorize(Roles = ...)] untuk aksi master data (HR + Support).
    public const string HrOrSupport = "HR,Support";

    // Nama policy untuk "atasan": bukan role, tapi claim IsManager dari Hierarchy.
    public const string ManagerOrHrPolicy = "ManagerOrHR";
    public const string IsManagerClaim = "IsManager";
}
