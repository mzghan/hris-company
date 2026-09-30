using System.Security.Claims;
using HRIS.Api.Common;
using HRIS.Api.Exceptions;

namespace HRIS.Api.Controllers;

public static class ClaimsPrincipalExtensions
{
    // Ambil employeeId dari JWT claim milik user yang sedang login.
    // Dipakai supaya Employee tidak bisa absen atau ajukan cuti
    // atas nama employee lain hanya dengan mengubah body request.
    public static int GetEmployeeId(this ClaimsPrincipal user)
    {
        var value = user.FindFirst("employeeId")?.Value;
        if (string.IsNullOrEmpty(value) || !int.TryParse(value, out var employeeId))
            throw new ForbiddenException("Akun ini tidak terhubung ke data Employee.");

        return employeeId;
    }

    // Ambil User.Id (bukan Employee.Id) dari JWT claim milik user yang
    // sedang login. Dipakai untuk field seperti EmployeeKpiScore.FilledByUserId
    // dan KpiScoreRevision.RevisedByUserId, yang sengaja menunjuk ke User
    // (bukan Employee) karena akun Support awal tidak selalu terhubung ke data
    // Employee — lihat User.EmployeeId yang nullable.
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(value) || !int.TryParse(value, out var userId))
            throw new ForbiddenException("Tidak bisa menentukan identitas user dari token.");

        return userId;
    }

    public static bool IsHrOrSupport(this ClaimsPrincipal user) =>
        user.IsInRole(RoleNames.HR) || user.IsInRole(RoleNames.Support);

    // Manager/Head/Group Head bukan role, tapi claim yang diisi saat login
    // kalau karyawan punya bawahan aktif di MST_Employee_Hierarchy.
    public static bool IsManager(this ClaimsPrincipal user) =>
        user.HasClaim(RoleNames.IsManagerClaim, "true");
}
