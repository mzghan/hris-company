namespace HRIS.Api.Common;

public static class RoleNames
{
    public const string Employee = "Employee";
    public const string HR = "HR";
    public const string Support = "Support";

    // Role khusus workflow JD & Manpower sesuai referensi HRManpower.
    public const string HRBP = "HRBP";
    public const string OE = "OE";
    public const string AVPOE = "AVP OE";
    public const string CHRO = "CHRO";
    public const string DoF = "DoF";
    public const string PresidentDirector = "President Director";
    public const string TA = "TA";
    public const string Admin = "Admin";
    public const string Manager = "Manager";
    public const string Head = "Head";
    public const string INTERN = "INTERN";

    // Dipakai di [Authorize(Roles = ...)] untuk aksi master data (HR + Support).
    public const string HrOrSupport = "HR,Support";

    // Nama policy untuk "atasan": bukan role, tapi claim IsManager dari Hierarchy.
    public const string ManagerOrHrPolicy = "ManagerOrHR";
    public const string IsManagerClaim = "IsManager";
}
