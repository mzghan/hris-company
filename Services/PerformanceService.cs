using HRIS.Api.Common;
using HRIS.Api.DTOs.Performance;
using HRIS.Api.Exceptions;
using HRIS.Api.Models;
using HRIS.Api.Repositories;
namespace HRIS.Api.Services;
public class PerformanceService : IPerformanceService
{
 private readonly IPerformanceRepository _repo; private readonly IEmployeeRepository _employees;
 public PerformanceService(IPerformanceRepository repo,IEmployeeRepository employees){_repo=repo;_employees=employees;}
 public async Task<List<PerformancePlanResponseDto>> GetPlansAsync(UserContext actor)
 { if(!actor.IsHrOrSupport&&actor.EmployeeId is null)throw new ForbiddenException("Akun ini tidak terhubung ke Employee."); return (await _repo.GetPlansAsync(actor.IsHrOrSupport?null:actor.EmployeeId)).Select(ToDto).ToList(); }
 public async Task<PerformancePlanResponseDto> CreatePlanAsync(PerformancePlanCreateDto dto,UserContext actor)
 {
   if(!actor.IsHrOrSupport&&actor.EmployeeId!=dto.EmployeeId&&!(actor.EmployeeId is int mid&&await _employees.IsDirectManagerOfAsync(mid,dto.EmployeeId)))throw new ForbiddenException("Hanya karyawan, atasan langsung, HR, atau Support yang boleh membuat performance plan.");
   if(dto.EndDate<dto.StartDate)throw new BadRequestException("End date harus setelah atau sama dengan start date.");
   _=await _employees.GetByIdAsync(dto.EmployeeId)??throw new NotFoundException("Employee tidak ditemukan.");
   var x=await _repo.AddPlanAsync(new PerformancePlan{EmployeeId=dto.EmployeeId,StartDate=dto.StartDate,EndDate=dto.EndDate,CreatedByUserId=actor.UserId,Status="Draft"});
   return ToDto(await _repo.GetPlanAsync(x.Id)!);
 }
 public async Task<PerformancePlanResponseDto> AddTaskAsync(PlanTaskCreateDto dto,UserContext actor)
 {
   var plan=await _repo.GetPlanAsync(dto.PlanId)??throw new NotFoundException("Performance plan tidak ditemukan."); await EnsureCanEditAsync(plan,actor);
   if(string.IsNullOrWhiteSpace(dto.Title))throw new BadRequestException("Task wajib diisi.");
   await _repo.AddTaskAsync(new PlanTask{PlanId=plan.Id,Title=dto.Title.Trim(),DueDate=dto.DueDate,AssignedByUserId=actor.UserId});
   return ToDto(await _repo.GetPlanAsync(plan.Id)!);
 }
 public async Task<PerformancePlanResponseDto> AddWorkLogAsync(PlanWorkLogCreateDto dto,UserContext actor)
 {
   var plan=await _repo.GetPlanAsync(dto.PlanId)??throw new NotFoundException("Performance plan tidak ditemukan.");
   if(actor.EmployeeId!=plan.EmployeeId&&!actor.IsHrOrSupport)throw new ForbiddenException("Work log hanya bisa diisi oleh pemilik plan, HR, atau Support.");
   if(string.IsNullOrWhiteSpace(dto.Description))throw new BadRequestException("Deskripsi work log wajib diisi.");
   await _repo.AddWorkLogAsync(new PlanWorkLog{PlanId=plan.Id,Description=dto.Description.Trim(),LoggedAt=DateTime.UtcNow});
   return ToDto(await _repo.GetPlanAsync(plan.Id)!);
 }
 public async Task<PerformancePlanResponseDto> UpdateTaskStatusAsync(int id,PlanTaskStatusDto dto,UserContext actor)
 {
   var plans=await _repo.GetPlansAsync(null);var plan=plans.FirstOrDefault(p=>p.Tasks.Any(t=>t.Id==id))??throw new NotFoundException("Task tidak ditemukan.");
   if(actor.EmployeeId!=plan.EmployeeId&&!actor.IsHrOrSupport)throw new ForbiddenException("Tidak punya akses.");
   var task=plan.Tasks.First(t=>t.Id==id); if(!new[]{"Open","InProgress","Done","Cancelled"}.Contains(dto.Status))throw new BadRequestException("Status task tidak valid.");task.Status=dto.Status;await _repo.UpdateTaskAsync(task);
   return ToDto(await _repo.GetPlanAsync(plan.Id)!);
 }
 private async Task EnsureCanEditAsync(PerformancePlan plan,UserContext actor){if(actor.IsHrOrSupport)return;if(actor.EmployeeId==plan.EmployeeId)return;if(actor.EmployeeId is int id&&await _employees.IsDirectManagerOfAsync(id,plan.EmployeeId))return;throw new ForbiddenException("Tidak punya akses ke performance plan ini.");}
 private static PerformancePlanResponseDto ToDto(PerformancePlan x)=>new(){Id=x.Id,EmployeeId=x.EmployeeId,EmployeeName=x.Employee?.FullName??"",StartDate=x.StartDate,EndDate=x.EndDate,Status=x.Status,CreatedByUserId=x.CreatedByUserId,CreatedByUsername=x.CreatedByUser?.Username??"",Tasks=x.Tasks.OrderBy(t=>t.DueDate).Select(t=>new PlanTaskDto{Id=t.Id,Title=t.Title,DueDate=t.DueDate,Status=t.Status}).ToList(),WorkLogs=x.WorkLogs.OrderByDescending(w=>w.LoggedAt).Select(w=>new PlanWorkLogDto{Id=w.Id,Description=w.Description,LoggedAt=w.LoggedAt}).ToList()};
}