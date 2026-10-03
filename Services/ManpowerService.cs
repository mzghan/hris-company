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
    private readonly IFileStorage _storage;
    public ManpowerService(IManpowerRepository repo,IEmployeeRepository employees,IReferenceRepository refs,IApprovalService approval,IFileStorage storage){_repo=repo;_employees=employees;_refs=refs;_approval=approval;_storage=storage;}
    public async Task<List<ManpowerRequestResponseDto>> GetAsync(UserContext actor)
    {
        if(!actor.IsHrOrSupport && actor.EmployeeId is null)throw new ForbiddenException("Akun ini tidak terhubung ke Employee.");
        var rows=await _repo.GetAllAsync(actor.IsManpowerAdmin || actor.IsTA?null:actor.EmployeeId);
        var apps=await _approval.GetByRefsAsync(ApprovalRequestTypes.Manpower,rows.Select(x=>x.Id),actor);
        return rows.Select(x=>ToDto(x,apps.GetValueOrDefault(x.Id)?.Id)).ToList();
    }
    public async Task<List<ManpowerVacancy>> GetVacanciesAsync(UserContext actor){ if(!actor.IsManpowerAdmin && !actor.IsTA) throw new ForbiddenException("Hanya HR/HRBP/OE/Admin/TA yang boleh melihat vacancy."); return await _repo.GetVacanciesAsync(); }
    public async Task AddFilingAsync(int id,string fileType,Microsoft.AspNetCore.Http.IFormFile file,UserContext actor)
    {
        if(actor.EmployeeId is null) throw new ForbiddenException("Akun tidak terhubung ke Employee.");
        var x=await _repo.GetByIdAsync(id)??throw new NotFoundException("Manpower request tidak ditemukan.");
        if(!actor.IsHrOrSupport && x.RequestedByEmployeeId!=actor.EmployeeId) throw new ForbiddenException("Kamu tidak punya akses ke request ini.");
        if(file.Length<=0) throw new BadRequestException("File kosong.");
        var ext=Path.GetExtension(file.FileName);
        if(string.IsNullOrWhiteSpace(ext)) throw new BadRequestException("File harus memiliki extension.");
        await using var stream=file.OpenReadStream();
        var stored=await _storage.SaveAsync(stream,ext);
        try { await _repo.AddFilingAsync(new ManpowerRequestFiling{ManpowerRequestId=id,FileType=fileType.Trim(),FileName=file.FileName,StoredPath=stored,ContentType=file.ContentType,SizeBytes=file.Length}); }
        catch { _storage.Delete(stored); throw; }
    }
    public async Task<ManpowerRequestResponseDto> CreateAsync(ManpowerRequestCreateDto dto,UserContext actor)
    {
        if(!actor.IsManpowerAdmin && !actor.IsManagerRole && !actor.IsHead && (actor.EmployeeId is null || !await _employees.HasActiveSubordinatesAsync(actor.EmployeeId.Value))) throw new ForbiddenException("Hanya Manager, HR, atau Support yang boleh mengajukan manpower request.");
        var emp=actor.EmployeeId??throw new ForbiddenException("Akun ini tidak terhubung ke Employee.");
        if(string.IsNullOrWhiteSpace(dto.Reason))throw new BadRequestException("Alasan wajib diisi.");
        if(dto.RequestType is not ("NewHeadcount" or "Replacement"))throw new BadRequestException("Request type tidak valid.");
        if(dto.RequestType=="Replacement" && dto.ReplacementForEmployeeId is null)throw new BadRequestException("Karyawan yang digantikan wajib dipilih untuk Replacement.");
        if(dto.WorkStatus is not ("FTE" or "Outsource"))throw new BadRequestException("Status pekerjaan harus FTE atau Outsource.");
        if(dto.TargetDate<DateOnly.FromDateTime(DateTime.Today))throw new BadRequestException("Target date tidak boleh di masa lalu.");
        if(!await _refs.ExistsAsync<Organization>(dto.OrganizationId)||!await _refs.ExistsAsync<JobTitle>(dto.JobTitleId)||!await _refs.ExistsAsync<JobLevel>(dto.JobLevelId)||!await _refs.ExistsAsync<EmploymentType>(dto.EmploymentTypeId))
            throw new BadRequestException("Referensi organisasi/jabatan/level/tipe pekerjaan tidak valid.");
        var x=await _repo.AddAsync(new ManpowerRequest{RequestedByEmployeeId=emp,RequestType=dto.RequestType,ReplacementForEmployeeId=dto.ReplacementForEmployeeId,ReportToEmployeeId=dto.ReportToEmployeeId,WorkStatus=dto.WorkStatus,JobDescriptionId=dto.JobDescriptionId,OrganizationId=dto.OrganizationId,JobTitleId=dto.JobTitleId,JobLevelId=dto.JobLevelId,EmploymentTypeId=dto.EmploymentTypeId,Headcount=dto.Headcount,Reason=dto.Reason.Trim(),TargetDate=dto.TargetDate,Status="Pending",ApprovalPhase="Waiting Approval"});
        var app=await _approval.SubmitAsync(ApprovalRequestTypes.Manpower,x.Id,emp,$"{dto.Headcount} tenaga kerja - {x.Reason}");
        return ToDto(await _repo.GetByIdAsync(x.Id)!,app.Id);
    }
    private static ManpowerRequestResponseDto ToDto(ManpowerRequest x,int? aid)=>new(){Id=x.Id,RequestedByEmployeeId=x.RequestedByEmployeeId,RequestedByName=x.RequestedByEmployee?.FullName??"",RequestType=x.RequestType,ReplacementForEmployeeId=x.ReplacementForEmployeeId,ReplacementForEmployeeName=x.ReplacementForEmployee?.FullName??"",ReportToEmployeeId=x.ReportToEmployeeId,ReportToEmployeeName=x.ReportToEmployee?.FullName??"",WorkStatus=x.WorkStatus,JobDescriptionId=x.JobDescriptionId,ApprovalPhase=x.ApprovalPhase,OrganizationId=x.OrganizationId,OrganizationName=x.Organization?.OrganizationName??"",JobTitleId=x.JobTitleId,JobTitleName=x.JobTitle?.JobTitleName??"",JobLevelId=x.JobLevelId,JobLevelName=x.JobLevel?.JobLevelName??"",EmploymentTypeId=x.EmploymentTypeId,EmploymentTypeName=x.EmploymentType?.EmploymentTypeName??"",Headcount=x.Headcount,Reason=x.Reason,TargetDate=x.TargetDate,Status=x.Status,ApprovalId=aid};
}
