using HRIS.Api.Common;
using HRIS.Api.DTOs.Competency;
using HRIS.Api.Exceptions;
using HRIS.Api.Models;
using HRIS.Api.Repositories;
namespace HRIS.Api.Services;
public class CompetencyService:ICompetencyService
{
 private readonly ICompetencyRepository _repo; private readonly IEmployeeRepository _employees;
 public CompetencyService(ICompetencyRepository repo,IEmployeeRepository employees){_repo=repo;_employees=employees;}
 public async Task<List<CompetencyResponseDto>> GetCompetenciesAsync()=>(await _repo.GetAllAsync()).Select(x=>new CompetencyResponseDto{Id=x.Id,Name=x.Name,Description=x.Description}).ToList();
 public async Task<List<EmployeeCompetencyResponseDto>> GetEmployeeCompetenciesAsync(UserContext actor)
 {if(!actor.IsHrOrSupport&&actor.EmployeeId is null)throw new ForbiddenException("Akun ini tidak terhubung ke Employee.");return (await _repo.GetEmployeeCompetenciesAsync(actor.IsHrOrSupport?null:actor.EmployeeId)).Select(ToDto).ToList();}
 public async Task<EmployeeCompetencyResponseDto> AssignAsync(EmployeeCompetencyCreateDto dto,UserContext actor)
 {
   if(!actor.IsHrOrSupport&&!(actor.EmployeeId is int mid&&await _employees.IsDirectManagerOfAsync(mid,dto.EmployeeId)))throw new ForbiddenException("Kompetensi hanya bisa ditetapkan oleh atasan langsung, HR, atau Support.");
   if(dto.TargetLevel<1||dto.TargetLevel>5)throw new BadRequestException("Target level harus 1-5.");
   _=await _employees.GetByIdAsync(dto.EmployeeId)??throw new NotFoundException("Employee tidak ditemukan.");_ = await _repo.GetByIdAsync(dto.CompetencyId)??throw new NotFoundException("Kompetensi tidak ditemukan.");
   var x=await _repo.AddEmployeeCompetencyAsync(new EmployeeCompetency{EmployeeId=dto.EmployeeId,CompetencyId=dto.CompetencyId,TargetLevel=dto.TargetLevel,AssignedByUserId=actor.UserId});
   return ToDto(await _repo.GetEmployeeCompetencyAsync(x.Id)!);
 }
 public async Task<EmployeeCompetencyResponseDto> AssessAsync(CompetencyAssessmentCreateDto dto,UserContext actor)
 {
   var x=await _repo.GetEmployeeCompetencyAsync(dto.EmployeeCompetencyId)??throw new NotFoundException("Kompetensi karyawan tidak ditemukan.");
   if(actor.EmployeeId!=x.EmployeeId&&!actor.IsHrOrSupport)throw new ForbiddenException("Self-assessment hanya boleh dilakukan oleh pemilik kompetensi, HR, atau Support.");
   if(dto.SelfLevel<1||dto.SelfLevel>5)throw new BadRequestException("Level harus 1-5.");
   await _repo.AddAssessmentAsync(new CompetencyAssessment{EmployeeCompetencyId=x.Id,SelfLevel=dto.SelfLevel,Note=dto.Note?.Trim()});
   return ToDto(await _repo.GetEmployeeCompetencyAsync(x.Id)!);
 }
 private static EmployeeCompetencyResponseDto ToDto(EmployeeCompetency x)=>new(){Id=x.Id,EmployeeId=x.EmployeeId,EmployeeName=x.Employee?.FullName??"",CompetencyId=x.CompetencyId,CompetencyName=x.Competency?.Name??"",TargetLevel=x.TargetLevel,Assessments=x.Assessments.OrderByDescending(a=>a.AssessedAt).Select(a=>new CompetencyAssessmentDto{Id=a.Id,SelfLevel=a.SelfLevel,Note=a.Note,AssessedAt=a.AssessedAt}).ToList()};
}