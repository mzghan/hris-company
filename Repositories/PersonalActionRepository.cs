using HRIS.Api.Data;
using HRIS.Api.Models;
using HRIS.Api.Common;
using Microsoft.EntityFrameworkCore;
namespace HRIS.Api.Repositories;
public class PersonalActionRepository : IPersonalActionRepository
{
    private readonly AppDbContext _db;
    public PersonalActionRepository(AppDbContext db)=>_db=db;
    private IQueryable<PersonalAction> Base()=>_db.PersonalActions
        .Include(x=>x.Employee).Include(x=>x.NewOrganization).Include(x=>x.NewJobTitle)
        .Include(x=>x.NewJobLevel).Include(x=>x.NewGrade).Include(x=>x.NewLocation).Include(x=>x.NewManager);
    public Task<List<PersonalAction>> GetAllAsync(int? employeeId)=>Base().Where(x=>employeeId==null||x.EmployeeId==employeeId).OrderByDescending(x=>x.CreatedAt).ToListAsync();
    public Task<PersonalAction?> GetByIdAsync(int id)=>Base().FirstOrDefaultAsync(x=>x.Id==id);
    public async Task<PersonalAction> AddAsync(PersonalAction x){_db.PersonalActions.Add(x);await _db.SaveChangesAsync();return x;}
    public async Task UpdateAsync(PersonalAction x)=>await _db.SaveChangesAsync();
    public Task<Employee?> GetEmployeeAsync(int id)=>_db.Employees.FirstOrDefaultAsync(x=>x.Id==id);
    public Task<EmployeeEmployment?> GetCurrentEmploymentAsync(int employeeId)=>_db.EmployeeEmployments.FirstOrDefaultAsync(x=>x.EmployeeId==employeeId&&x.EndDate==null);
    public Task<EmployeeHierarchy?> GetCurrentDirectManagerAsync(int employeeId)=>_db.EmployeeHierarchies.Include(x=>x.HierarchyType).FirstOrDefaultAsync(x=>x.EmployeeId==employeeId&&x.EndDate==null&&x.HierarchyType!.HierarchyTypeName==RefNames.DirectManager);
    public async Task ApplyApprovedAsync(PersonalAction a)
    {
        var current=await GetCurrentEmploymentAsync(a.EmployeeId);
        var employmentChanged=a.NewOrganizationId is not null||a.NewJobTitleId is not null||a.NewJobLevelId is not null||a.NewGradeId is not null||a.NewLocationId is not null;
        if(employmentChanged)
        {
            if(current is null) throw new InvalidOperationException("Employment aktif karyawan tidak ditemukan.");
            current.EndDate=a.EffectiveDate.AddDays(-1);
            await _db.SaveChangesAsync();
            _db.EmployeeEmployments.Add(new EmployeeEmployment
            {
                EmployeeId=a.EmployeeId, EmploymentStatusId=current.EmploymentStatusId, EmploymentTypeId=current.EmploymentTypeId,
                VendorId=current.VendorId, OrganizationId=a.NewOrganizationId??current.OrganizationId,
                LocationId=a.NewLocationId??current.LocationId, JobLevelId=a.NewJobLevelId??current.JobLevelId,
                JobTitleId=a.NewJobTitleId??current.JobTitleId, GradeId=a.NewGradeId??current.GradeId,
                IsFte=current.IsFte, IsSales=current.IsSales, ContractEndDate=current.ContractEndDate, StartDate=a.EffectiveDate
            });
            await _db.SaveChangesAsync();
        }
        if(a.NewManagerId is not null)
        {
            var old=await GetCurrentDirectManagerAsync(a.EmployeeId);
            if(old is not null)
            {
                old.EndDate=a.EffectiveDate.AddDays(-1);
                await _db.SaveChangesAsync();
            }
            _db.EmployeeHierarchies.Add(new EmployeeHierarchy
            {
                EmployeeId=a.EmployeeId, ManagerId=a.NewManagerId.Value,
                HierarchyTypeId=old?.HierarchyTypeId??await _db.HierarchyTypes.Where(x=>x.HierarchyTypeName==RefNames.DirectManager).Select(x=>x.Id).FirstAsync(),
                StartDate=a.EffectiveDate
            });
            await _db.SaveChangesAsync();
        }
        a.Status="Approved";
        await _db.SaveChangesAsync();
    }
}