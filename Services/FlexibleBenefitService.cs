using HRIS.Api.Common;
using HRIS.Api.DTOs.FlexibleBenefit;
using HRIS.Api.Exceptions;
using HRIS.Api.Models;
using HRIS.Api.Repositories;

namespace HRIS.Api.Services;

public class FlexibleBenefitService : IFlexibleBenefitService
{
    private readonly IFlexibleBenefitRepository _repo;
    private readonly IEmployeeRepository _employees;
    private readonly IReferenceRepository _refs;
    private readonly IApprovalService _approval;
    private readonly IAuditLogService _audit;

    public FlexibleBenefitService(IFlexibleBenefitRepository repo, IEmployeeRepository employees, IReferenceRepository refs, IApprovalService approval, IAuditLogService audit)
    { _repo=repo; _employees=employees; _refs=refs; _approval=approval; _audit=audit; }

    public async Task<List<LeaveBalanceResponseDto>> GetBalancesAsync(UserContext actor, int year)
    {
        var employeeId = actor.EmployeeId ?? throw new ForbiddenException("Akun ini tidak terhubung ke Employee.");
        var rows = await _repo.GetBalancesAsync(employeeId, year);
        return rows.Select(ToDto).ToList();
    }

    public async Task<List<FlexPeriodResponseDto>> GetPeriodsAsync(bool openOnly=false) =>
        (await _repo.GetPeriodsAsync(openOnly)).Select(x=>new FlexPeriodResponseDto
        { Id=x.Id, Name=x.Name, StartDate=x.StartDate, EndDate=x.EndDate, IsOpen=x.IsOpen }).ToList();

    public async Task<List<LeaveEncashmentResponseDto>> GetEncashmentsAsync(UserContext actor)
    {
        var rows = await _repo.GetEncashmentsAsync(actor.IsHrOrSupport ? null : actor.EmployeeId);
        var apps = await _approval.GetByRefsAsync(ApprovalRequestTypes.LeaveEncashment, rows.Select(x=>x.Id), actor);
        return rows.Select(x=>new LeaveEncashmentResponseDto
        {
            Id=x.Id, EmployeeId=x.EmployeeId, EmployeeName=x.Employee?.FullName??"",
            PeriodId=x.PeriodId, PeriodName=x.Period?.Name??"", LeaveTypeId=x.LeaveTypeId,
            LeaveTypeName=x.LeaveType?.Name??"", Days=x.Days, Status=x.Status,
            ApprovalId=apps.GetValueOrDefault(x.Id)?.Id
        }).ToList();
    }

    public async Task<LeaveEncashmentResponseDto> CreateEncashmentAsync(LeaveEncashmentCreateDto dto, UserContext actor)
    {
        var employeeId = actor.EmployeeId ?? throw new ForbiddenException("Akun ini tidak terhubung ke Employee.");
        var emp = await _employees.GetByIdAsync(employeeId) ?? throw new NotFoundException("Employee tidak ditemukan.");
        var current = emp.Employments.FirstOrDefault(x=>x.EndDate==null);
        if (current is not null && !await _refs.IsModuleAllowedAsync(current.EmploymentTypeId, "6"))
            throw new ForbiddenException("Karyawan dengan tipe pekerjaan saat ini tidak mendapatkan Flexible Benefit.");

        var period = await _repo.GetPeriodAsync(dto.PeriodId) ?? throw new BadRequestException("Periode Flexible Benefit tidak ditemukan.");
        if (!period.IsOpen)
            throw new BadRequestException("Periode Flexible Benefit tidak terbuka.");
        var type = await _repo.GetLeaveTypeAsync(dto.LeaveTypeId) ?? throw new BadRequestException("Jenis cuti tidak ditemukan.");
        if (!type.IsSellable) throw new BadRequestException("Jenis cuti ini tidak dapat dijual.");

        var balance = await _repo.GetBalanceAsync(employeeId, dto.LeaveTypeId, period.StartDate.Year)
            ?? throw new BadRequestException("Saldo cuti untuk tahun periode belum tersedia.");
        var pending = (await _repo.GetEncashmentsAsync(employeeId))
            .Where(x=>x.LeaveTypeId==dto.LeaveTypeId && x.Status=="Pending").Sum(x=>x.Days);
        var available = balance.Entitlement + balance.CarriedOver - balance.Used - balance.Sold - pending;
        if (dto.Days > available) throw new BadRequestException($"Sisa yang dapat dijual hanya {Math.Max(available,0)} hari.");

        var x=await _repo.AddEncashmentAsync(new LeaveEncashment
        { EmployeeId=employeeId, PeriodId=period.Id, LeaveTypeId=type.Id, Days=dto.Days, Status="Pending" });
        var app=await _approval.SubmitAsync(ApprovalRequestTypes.LeaveEncashment,x.Id,employeeId,$"Penjualan {dto.Days} hari {type.Name}");
        return (await GetEncashmentsAsync(actor)).First(x=>x.Id==app.RequestRefId);
    }

    public async Task<List<HealthClaimResponseDto>> GetHealthClaimsAsync(UserContext actor)
    {
        var rows=await _repo.GetClaimsAsync(actor.IsHrOrSupport?null:actor.EmployeeId);
        var apps=await _approval.GetByRefsAsync(ApprovalRequestTypes.HealthClaim, rows.Select(x=>x.Id), actor);
        return rows.Select(x=>new HealthClaimResponseDto
        { Id=x.Id, EmployeeId=x.EmployeeId, EmployeeName=x.Employee?.FullName??"", PeriodId=x.PeriodId,
          PeriodName=x.Period?.Name??"", Amount=x.Amount, Description=x.Description, Status=x.Status,
          ApprovalId=apps.GetValueOrDefault(x.Id)?.Id }).ToList();
    }

    public async Task<HealthClaimResponseDto> CreateHealthClaimAsync(HealthClaimCreateDto dto, UserContext actor)
    {
        var employeeId=actor.EmployeeId ?? throw new ForbiddenException("Akun ini tidak terhubung ke Employee.");
        var emp=await _employees.GetByIdAsync(employeeId) ?? throw new NotFoundException("Employee tidak ditemukan.");
        var current=emp.Employments.FirstOrDefault(x=>x.EndDate==null);
        if (current is not null && !await _refs.IsModuleAllowedAsync(current.EmploymentTypeId,"6"))
            throw new ForbiddenException("Karyawan dengan tipe pekerjaan saat ini tidak mendapatkan Flexible Benefit.");
        var period=await _repo.GetPeriodAsync(dto.PeriodId) ?? throw new BadRequestException("Periode Flexible Benefit tidak ditemukan.");
        if (!period.IsOpen) throw new BadRequestException("Periode Flexible Benefit tidak terbuka.");
        var x=await _repo.AddClaimAsync(new HealthClaim
        { EmployeeId=employeeId, PeriodId=period.Id, Amount=dto.Amount, Description=dto.Description?.Trim(), Status="Pending" });
        var app=await _approval.SubmitAsync(ApprovalRequestTypes.HealthClaim,x.Id,employeeId,$"Klaim kesehatan Rp {dto.Amount:N0}");
        return (await GetHealthClaimsAsync(actor)).First(x=>x.Id==app.RequestRefId);
    }


    public async Task UpdateHealthClaimStatusAsync(int id, string status, UserContext actor)
    {
        if (!actor.IsHrOrSupport) throw new ForbiddenException("Hanya HR/Support yang boleh mengubah status klaim.");
        var x = await _repo.GetClaimAsync(id) ?? throw new NotFoundException("Klaim kesehatan tidak ditemukan.");
        if (status is not ("Approved" or "Rejected" or "Paid"))
            throw new BadRequestException("Status klaim hanya Approved, Rejected, atau Paid.");
        if (status == "Paid" && x.Status != "Approved")
            throw new BadRequestException("Klaim hanya bisa ditandai Paid setelah Approved.");
        x.Status = status;
        await _repo.SaveAsync();
        if(actor.IsSupport) await _audit.LogAsync(actor.UserId,"HealthClaim.Status",nameof(HealthClaim),id,$"Status menjadi {status}.");
    }

    private static LeaveBalanceResponseDto ToDto(LeaveBalance x)=>new()
    { Id=x.Id, EmployeeId=x.EmployeeId, LeaveTypeId=x.LeaveTypeId, LeaveTypeName=x.LeaveType?.Name??"",
      Year=x.Year, Entitlement=x.Entitlement, Used=x.Used, Sold=x.Sold, CarriedOver=x.CarriedOver };
}
