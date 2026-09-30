using HRIS.Api.Models;
namespace HRIS.Api.Repositories;
public interface IFamilyChangeRepository { Task<List<FamilyChangeRequest>> GetAllAsync(int? employeeId = null); Task<FamilyChangeRequest?> GetByIdAsync(int id); Task<FamilyChangeRequest> AddAsync(FamilyChangeRequest x); Task UpdateAsync(FamilyChangeRequest x); Task<EmployeeFamily?> GetFamilyAsync(int employeeId,int familyId); }
