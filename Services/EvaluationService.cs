using HRIS.Api.Common;
using HRIS.Api.DTOs.Evaluation;
using HRIS.Api.Exceptions;
using HRIS.Api.Models;
using HRIS.Api.Repositories;
namespace HRIS.Api.Services;
public class EvaluationService : IEvaluationService
{
 private readonly IEvaluationRepository _repo; private readonly IEmployeeRepository _employees; private readonly IReferenceRepository _refs;
 public EvaluationService(IEvaluationRepository repo,IEmployeeRepository employees,IReferenceRepository refs){_repo=repo;_employees=employees;_refs=refs;}
 public async Task<List<EvaluationResponseDto>> GetAsync(UserContext actor)
 {
   if(!actor.IsHrOrSupport&&actor.EmployeeId is null) throw new ForbiddenException("Akun ini tidak terhubung ke Employee.");
   await EnsureScheduledAsync();
   var rows=await _repo.GetAllAsync(actor.IsHrOrSupport?null:actor.EmployeeId);
   foreach(var x in rows) if(x.Status=="Pending" && x.DueDate<JakartaTime.Today()) x.Status="Overdue";
   return rows.Select(ToDto).ToList();
 }
 public async Task<EvaluationResponseDto> GetByIdAsync(int id,UserContext actor)
 {
   var x=await _repo.GetByIdAsync(id)??throw new NotFoundException("Evaluasi tidak ditemukan.");
   await EnsureViewAsync(x,actor); if(x.Status=="Pending"&&x.DueDate<JakartaTime.Today())x.Status="Overdue";
   return ToDto(x);
 }
 public async Task<EvaluationResponseDto> AddEntryAsync(int id,EvaluationEntryCreateDto dto,UserContext actor)
 {
   var x=await _repo.GetByIdAsync(id)??throw new NotFoundException("Evaluasi tidak ditemukan.");
   await EnsureViewAsync(x,actor);
   if(string.IsNullOrWhiteSpace(dto.WorkDescription))throw new BadRequestException("Deskripsi pekerjaan wajib diisi.");
   if(actor.EmployeeId is null)throw new ForbiddenException("Akun ini tidak terhubung ke Employee.");
   await _repo.AddEntryAsync(new EvaluationEntry{EvaluationId=id,WorkDescription=dto.WorkDescription.Trim()});
   return ToDto(await _repo.GetByIdAsync(id)!);
 }
 public async Task<EvaluationResponseDto> AddScoreAsync(int id,EvaluationScoreCreateDto dto,UserContext actor)
 {
   var x=await _repo.GetByIdAsync(id)??throw new NotFoundException("Evaluasi tidak ditemukan.");
   if(!actor.IsHrOrSupport && (actor.EmployeeId is null || !await _employees.IsDirectManagerOfAsync(actor.EmployeeId.Value,x.EmployeeId)))
      throw new ForbiddenException("Hanya atasan langsung, HR, atau Support yang boleh memberi nilai.");
   if(dto.Score<0||dto.Score>100)throw new BadRequestException("Nilai harus 0 sampai 100.");
   await _repo.AddScoreAsync(new EvaluationScore{EvaluationId=id,ScoredByUserId=actor.UserId,Score=dto.Score,Note=dto.Note?.Trim()});
   x.Status="Completed";x.CompletedAt=DateTime.UtcNow;await _repo.UpdateAsync(x);
   return ToDto(await _repo.GetByIdAsync(id)!);
 }
 private async Task EnsureScheduledAsync()
 {
   var types=await _repo.GetTypesAsync(); var employments=await _repo.GetActiveEmploymentsAsync();
   foreach(var e in employments)
   {
     foreach(var t in types)
     {
       var isProbation=t.Name.StartsWith("Probation",StringComparison.OrdinalIgnoreCase)
           && (e.EmploymentStatus?.EmploymentStatusName?.Equals("Probation",StringComparison.OrdinalIgnoreCase)??false);
       var isContract=t.Name.Equals("Contract Based",StringComparison.OrdinalIgnoreCase)
           && (e.EmploymentType?.EmploymentTypeName?.Equals("Contract",StringComparison.OrdinalIgnoreCase)??false);
       if(!isProbation&&!isContract)continue;
       var existing=(await _repo.GetAllAsync(e.EmployeeId)).Any(x=>x.EmploymentId==e.Id&&x.EvaluationTypeId==t.Id);
       if(existing)continue;
       var due=isProbation&&t.MonthOffset is int months ? e.StartDate.AddMonths(months) : e.ContractEndDate ?? e.StartDate;
       await _repo.AddAsync(new EmployeeEvaluation{EmployeeId=e.EmployeeId,EmploymentId=e.Id,EvaluationTypeId=t.Id,DueDate=due,Status="Pending"});
     }
   }
 }
 private async Task EnsureViewAsync(EmployeeEvaluation x,UserContext actor)
 {
   if(actor.IsHrOrSupport)return;
   if(actor.EmployeeId==x.EmployeeId)return;
   if(actor.EmployeeId is not null&&await _employees.IsDirectManagerOfAsync(actor.EmployeeId.Value,x.EmployeeId))return;
   throw new ForbiddenException("Kamu tidak punya akses ke evaluasi ini.");
 }
 private static EvaluationResponseDto ToDto(EmployeeEvaluation x)=>new(){Id=x.Id,EmployeeId=x.EmployeeId,EmployeeName=x.Employee?.FullName??"",EmploymentId=x.EmploymentId,EvaluationTypeId=x.EvaluationTypeId,EvaluationTypeName=x.EvaluationType?.Name??"",DueDate=x.DueDate,Status=x.Status,CompletedAt=x.CompletedAt,Entries=x.Entries.OrderByDescending(e=>e.CreatedAt).Select(e=>new EvaluationEntryDto{Id=e.Id,WorkDescription=e.WorkDescription,CreatedAt=e.CreatedAt}).ToList(),Scores=x.Scores.OrderByDescending(s=>s.ScoredAt).Select(s=>new EvaluationScoreDto{Id=s.Id,ScoredByUserId=s.ScoredByUserId,ScoredByUsername=s.ScoredByUser?.Username??"",Score=s.Score,Note=s.Note,ScoredAt=s.ScoredAt}).ToList()};
}