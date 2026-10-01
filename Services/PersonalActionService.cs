using HRIS.Api.Common;
using HRIS.Api.DTOs.PersonalAction;
using HRIS.Api.Exceptions;
using HRIS.Api.Models;
using HRIS.Api.Repositories;
namespace HRIS.Api.Services;
public class PersonalActionService:IPersonalActionService
{
 private readonly IPersonalActionRepository _repo; private readonly IReferenceRepository _refs; private readonly IEmployeeRepository _employees; private readonly IApprovalService _approval;
 public PersonalActionService(IPersonalActionRepository repo,IReferenceRepository refs,IEmployeeRepository employees,IApprovalService approval){_repo=repo;_refs=refs;_employees=employees;_approval=approval;}
 public async Task<List<PersonalActionResponseDto>> GetAsync(UserContext actor)
 {if(!actor.IsHrOrSupport&&actor.EmployeeId is null)throw new ForbiddenException("Akun ini tidak terhubung ke Employee.");var rows=await _repo.GetAllAsync(actor.IsHrOrSupport?null:actor.EmployeeId);var apps=await _approval.GetByRefsAsync(ApprovalRequestTypes.PersonalAction,rows.Select(x=>x.Id),actor);return rows.Select(x=>ToDto(x,apps.GetValueOrDefault(x.Id)?.Id)).ToList();}
 public async Task<PersonalActionResponseDto> CreateAsync(PersonalActionCreateDto dto,UserContext actor)
 {
   if(!actor.IsHrOrSupport&&dto.EmployeeId!=actor.EmployeeId)throw new ForbiddenException("Karyawan hanya boleh membuat PAF untuk dirinya sendiri.");
   if(!new[]{"GradeChange","Transfer","InfoChange"}.Contains(dto.ActionType))throw new BadRequestException("ActionType harus GradeChange, Transfer, atau InfoChange.");
   if(dto.EffectiveDate<DateOnly.FromDateTime(DateTime.Today))throw new BadRequestException("Effective date tidak boleh di masa lalu.");
   if(string.IsNullOrWhiteSpace(dto.Reason))throw new BadRequestException("Alasan wajib diisi.");
   var employee=await _repo.GetEmployeeAsync(dto.EmployeeId)??throw new NotFoundException("Employee tidak ditemukan.");
   var current=await _repo.GetCurrentEmploymentAsync(dto.EmployeeId);
   if(current is null)throw new BadRequestException("Employee tidak memiliki employment aktif.");
   if(dto.EffectiveDate<=current.StartDate)throw new BadRequestException("Effective date harus setelah tanggal mulai employment saat ini.");
   await ValidateRefsAsync(dto);
   if(dto.NewManagerId==dto.EmployeeId)throw new BadRequestException("Employee tidak bisa menjadi manager untuk dirinya sendiri.");
   if(dto.NewManagerId is int nm)
   {
     var newManager=await _repo.GetEmployeeAsync(nm)??throw new BadRequestException("New manager tidak ditemukan.");
     if(!newManager.IsActive)throw new BadRequestException("New manager harus aktif.");
     var oldManager=await _repo.GetCurrentDirectManagerAsync(dto.EmployeeId);
     if(oldManager?.ManagerId==nm)throw new BadRequestException("Manager baru sama dengan manager saat ini.");
     var chain=await _employees.GetManagerChainAsync(nm,50);
     if(chain.Any(x=>x.Id==dto.EmployeeId))throw new BadRequestException("New manager berada di bawah employee ini dan akan membuat siklus.");
   }
   if(dto.NewOrganizationId is null&&dto.NewJobTitleId is null&&dto.NewJobLevelId is null&&dto.NewGradeId is null&&dto.NewLocationId is null&&dto.NewManagerId is null)throw new BadRequestException("PAF harus memiliki minimal satu perubahan.");
   var x=await _repo.AddAsync(new PersonalAction{EmployeeId=dto.EmployeeId,ActionType=dto.ActionType,EffectiveDate=dto.EffectiveDate,Reason=dto.Reason.Trim(),Status="Pending",NewOrganizationId=dto.NewOrganizationId,NewJobTitleId=dto.NewJobTitleId,NewJobLevelId=dto.NewJobLevelId,NewGradeId=dto.NewGradeId,NewLocationId=dto.NewLocationId,NewManagerId=dto.NewManagerId});
   var old=await _repo.GetCurrentDirectManagerAsync(dto.EmployeeId);
   var app=await _approval.SubmitPersonalActionAsync(x.Id,dto.EmployeeId,old?.ManagerId,dto.NewManagerId,$"{dto.ActionType} efektif {dto.EffectiveDate:dd/MM/yyyy}");
   return ToDto(await _repo.GetByIdAsync(x.Id)!,app.Id);
 }
 private async Task ValidateRefsAsync(PersonalActionCreateDto dto)
 {if(dto.NewOrganizationId is int a&&!await _refs.ExistsAsync<Organization>(a))throw new BadRequestException("Organisasi tidak ditemukan.");if(dto.NewJobTitleId is int b&&!await _refs.ExistsAsync<JobTitle>(b))throw new BadRequestException("Jabatan tidak ditemukan.");if(dto.NewJobLevelId is int c&&!await _refs.ExistsAsync<JobLevel>(c))throw new BadRequestException("Job level tidak ditemukan.");if(dto.NewGradeId is int d&&!await _refs.ExistsAsync<Grade>(d))throw new BadRequestException("Grade tidak ditemukan.");if(dto.NewLocationId is int e&&!await _refs.ExistsAsync<Location>(e))throw new BadRequestException("Lokasi tidak ditemukan.");}
 private static PersonalActionResponseDto ToDto(PersonalAction x,int? aid)=>new(){Id=x.Id,EmployeeId=x.EmployeeId,EmployeeName=x.Employee?.FullName??"",ActionType=x.ActionType,EffectiveDate=x.EffectiveDate,Reason=x.Reason,Status=x.Status,NewOrganizationId=x.NewOrganizationId,NewOrganizationName=x.NewOrganization?.OrganizationName,NewJobTitleId=x.NewJobTitleId,NewJobTitleName=x.NewJobTitle?.JobTitleName,NewJobLevelId=x.NewJobLevelId,NewJobLevelName=x.NewJobLevel?.JobLevelName,NewGradeId=x.NewGradeId,NewGradeLevel=x.NewGrade?.GradeLevel,NewLocationId=x.NewLocationId,NewLocationName=x.NewLocation?.LocationName,NewManagerId=x.NewManagerId,NewManagerName=x.NewManager?.FullName,ApprovalId=aid};
}