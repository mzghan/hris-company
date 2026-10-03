using System.Text.Json;
using HRIS.Api.Common;
using HRIS.Api.DTOs.JobDescription;
using HRIS.Api.Exceptions;
using HRIS.Api.Models;
using HRIS.Api.Repositories;
namespace HRIS.Api.Services;
public class JobDescriptionService : IJobDescriptionService
{
    private readonly IJobDescriptionRepository _repo;
    private readonly IApprovalService _approval;
    public JobDescriptionService(IJobDescriptionRepository repo,IApprovalService approval){_repo=repo;_approval=approval;}
    public async Task<List<JobDescriptionResponseDto>> GetAsync(UserContext actor)
    {
        if(!actor.IsHrOrSupport && actor.EmployeeId is null) throw new ForbiddenException("Akun tidak terhubung ke Employee.");
        var rows = await _repo.GetAllAsync();
        if (!actor.IsManpowerAdmin && actor.EmployeeId is int employeeId)
            rows = rows.Where(x => x.JobHolderEmployeeId == employeeId || x.ImmediateManagerEmployeeId == employeeId).ToList();
        return rows.Select(ToDto).ToList();
    }
    public async Task<JobDescriptionResponseDto> GetByIdAsync(int id, UserContext actor){var x=await _repo.GetByIdAsync(id)??throw new NotFoundException("JD tidak ditemukan.");return ToDto(x);}
    public async Task<JobDescriptionResponseDto> CreateAsync(JobDescriptionCreateDto dto, UserContext actor, bool submit)
    {
        if(actor.EmployeeId is null) throw new ForbiddenException("JD membutuhkan akun yang terhubung ke Employee.");
        if(!actor.IsHrOrSupport && actor.EmployeeId is null) throw new ForbiddenException("Hanya HR/Manager yang dapat membuat JD.");
        var code=$"JD-{DateTime.UtcNow:yyyyMMddHHmmssfff}";
        var x=new JobDescription{Code=code,JobTitle=dto.JobTitle.Trim(),OrganizationId=dto.OrganizationId,JobLevelId=dto.JobLevelId,GradeId=dto.GradeId,JobHolderEmployeeId=dto.JobHolderEmployeeId,ImmediateManagerEmployeeId=dto.ImmediateManagerEmployeeId,
            Purpose=dto.Purpose,ReportingRelationship=dto.ReportingRelationship,Dimensions=dto.Dimensions,KriCico=dto.KriCico,KriCompliance=dto.KriCompliance,KriAreas=dto.KriAreas,
            Stakeholders=dto.Stakeholders,Challenges=dto.Challenges,Qualifications=dto.Qualifications,Experience=dto.Experience,Competencies=dto.Competencies,
            Status=submit?"Submitted":"Draft",ApprovalFlag=submit?0:9};
        await _repo.AddAsync(x);
        var snapshot=JsonSerializer.Serialize(dto);
        x.Revisions.Add(new JobDescriptionRevision{RevisionNo=1,SnapshotJson=snapshot,ChangeReason=submit?"Submitted":"Draft"});
        await _repo.AddBankAsync(new JobDescriptionBank{Code=x.Code,JobTitle=x.JobTitle,SnapshotJson=snapshot,Status=submit?"Submitted":"Template"});
        await _repo.UpdateAsync(x);
        if(submit) await _approval.SubmitAsync("JobDescription",x.Id,actor.EmployeeId.Value,$"JD {x.JobTitle}");
        return ToDto((await _repo.GetByIdAsync(x.Id))!);
    }
    public async Task<JobDescriptionResponseDto> SignJobHolderAsync(int id,string signature,UserContext actor)
    {
        var x=await _repo.GetByIdAsync(id)??throw new NotFoundException("JD tidak ditemukan.");
        if(actor.EmployeeId is null || x.JobHolderEmployeeId!=actor.EmployeeId) throw new ForbiddenException("Bukan Job Holder JD ini.");
        x.JobHolderSignatureText=signature.Trim();x.DateSignJobHolder=DateTime.UtcNow;x.ApprovalFlag=6;x.Status="Awaiting Manager Signature";await _repo.UpdateAsync(x);return ToDto(x);
    }
    public async Task<JobDescriptionResponseDto> SignManagerAsync(int id,string signature,UserContext actor)
    {
        var x=await _repo.GetByIdAsync(id)??throw new NotFoundException("JD tidak ditemukan.");
        if(actor.EmployeeId is null || x.ImmediateManagerEmployeeId!=actor.EmployeeId) throw new ForbiddenException("Bukan Immediate Manager JD ini.");
        x.ManagerSignatureText=signature.Trim();x.DateSignManager=DateTime.UtcNow;x.ApprovalFlag=7;x.Status="Finalized";await _repo.UpdateAsync(x);
        await _repo.UpsertMasterAsync(new JobDescriptionMaster{Code=x.Code,JobTitle=x.JobTitle,SnapshotJson=JsonSerializer.Serialize(new { x.Code,x.JobTitle,x.Purpose,x.ReportingRelationship,x.Dimensions,x.KriCico,x.KriCompliance,x.KriAreas,x.Stakeholders,x.Challenges,x.Qualifications,x.Experience,x.Competencies }),FinalizedAt=DateTime.UtcNow,JobHolderEmployeeId=x.JobHolderEmployeeId,ImmediateManagerEmployeeId=x.ImmediateManagerEmployeeId});
        return ToDto(x);
    }
    private static JobDescriptionResponseDto ToDto(JobDescription x)=>new(){Id=x.Id,Code=x.Code,JobTitle=x.JobTitle,OrganizationName=x.Organization?.OrganizationName??"",JobLevelName=x.JobLevel?.JobLevelName??"",GradeName=x.Grade?.GradeDescription??"",Status=x.Status,ApprovalFlag=x.ApprovalFlag,Purpose=x.Purpose,ReportingRelationship=x.ReportingRelationship,Dimensions=x.Dimensions,KriCico=x.KriCico,KriCompliance=x.KriCompliance,KriAreas=x.KriAreas,Stakeholders=x.Stakeholders,Challenges=x.Challenges,Qualifications=x.Qualifications,Experience=x.Experience,Competencies=x.Competencies,JobHolderSignatureText=x.JobHolderSignatureText,ManagerSignatureText=x.ManagerSignatureText};
}
