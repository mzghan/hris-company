using HRIS.Api.Common;
using HRIS.Api.DTOs.Manpower;
using HRIS.Api.Exceptions;
using HRIS.Api.Models;
using HRIS.Api.Repositories;
namespace HRIS.Api.Services;
public class ManpowerService : IManpowerService
{
    private readonly IManpowerRepository _repo;
    private readonly IEmployeeRepository _employees;
    private readonly IReferenceRepository _refs;
    private readonly IApprovalService _approval;
    public ManpowerService(IManpowerRepository repo,IEmployeeRepository employees,IReferenceRepository refs,IApprovalService approval){_repo=repo;_employees=employees;_refs=refs;_approval=approval;}
    public async Task<List<ManpowerRequestResponseDto>> GetAsync(UserContext actor)
    {
        if(!actor.IsHrOrSupport && actor.EmployeeId is null)throw new ForbiddenException("Akun ini tidak terhubung ke Employee.");
        var rows=await _repo.GetAllAsync(actor.IsHrOrSupport?null:actor.EmployeeId);
        var apps=await _approval.GetByRefsAsync(ApprovalRequestTypes.Manpower,rows.Select(x=>x.Id),actor);
        return rows.Select(x=>ToDto(x,apps.GetValueOrDefault(x.Id)?.Id)).ToList();
    }
    public async Task<ManpowerRequestResponseDto> CreateAsync(ManpowerRequestCreateDto dto,UserContext actor)
    {
        if(!actor.IsHrOrSupport && (actor.EmployeeId is null || !await _employees.HasActiveSubordinatesAsync(actor.EmployeeId.Value))) throw new ForbiddenException("Hanya Manager, HR, atau Support yang boleh mengajukan manpower request.");
        var emp=actor.EmployeeId??throw new ForbiddenException("Akun ini tidak terhubung ke Employee.");
        if(string.IsNullOrWhiteSpace(dto.Reason))throw new BadRequestException("Alasan wajib diisi.");
        if(dto.TargetDate<DateOnly.FromDateTime(DateTime.Today))throw new BadRequestException("Target date tidak boleh di masa lalu.");
        if(!await _refs.ExistsAsync<Organization>(dto.OrganizationId)||!await _refs.ExistsAsync<JobTitle>(dto.JobTitleId)||!await _refs.ExistsAsync<JobLevel>(dto.JobLevelId)||!await _refs.ExistsAsync<EmploymentType>(dto.EmploymentTypeId))
            throw new BadRequestException("Referensi organisasi/jabatan/level/tipe pekerjaan tidak valid.");
        var x=await _repo.AddAsync(new ManpowerRequest{RequestedByEmployeeId=emp,OrganizationId=dto.OrganizationId,JobTitleId=dto.JobTitleId,JobLevelId=dto.JobLevelId,EmploymentTypeId=dto.EmploymentTypeId,Headcount=dto.Headcount,Reason=dto.Reason.Trim(),TargetDate=dto.TargetDate,Status="Pending"});
        var app=await _approval.SubmitAsync(ApprovalRequestTypes.Manpower,x.Id,emp,$"{dto.Headcount} tenaga kerja - {x.Reason}");
        return ToDto(await _repo.GetByIdAsync(x.Id)!,app.Id);
    }
    private static ManpowerRequestResponseDto ToDto(ManpowerRequest x,int? aid)=>new(){Id=x.Id,RequestedByEmployeeId=x.RequestedByEmployeeId,RequestedByName=x.RequestedByEmployee?.FullName??"",OrganizationId=x.OrganizationId,OrganizationName=x.Organization?.OrganizationName??"",JobTitleId=x.JobTitleId,JobTitleName=x.JobTitle?.JobTitleName??"",JobLevelId=x.JobLevelId,JobLevelName=x.JobLevel?.JobLevelName??"",EmploymentTypeId=x.EmploymentTypeId,EmploymentTypeName=x.EmploymentType?.EmploymentTypeName??"",Headcount=x.Headcount,Reason=x.Reason,TargetDate=x.TargetDate,Status=x.Status,ApprovalId=aid};
}
