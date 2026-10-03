using HRIS.Api.Data;
using HRIS.Api.Models;
using Microsoft.EntityFrameworkCore;
namespace HRIS.Api.Repositories;
public class JobDescriptionRepository : IJobDescriptionRepository
{
    private readonly AppDbContext _db;
    public JobDescriptionRepository(AppDbContext db)=>_db=db;
    private IQueryable<JobDescription> Base()=>_db.JobDescriptions.Include(x=>x.Organization).Include(x=>x.JobLevel).Include(x=>x.Grade).OrderByDescending(x=>x.CreatedAt);
    public Task<List<JobDescription>> GetAllAsync()=>Base().ToListAsync();
    public Task<JobDescription?> GetByIdAsync(int id)=>Base().Include(x=>x.Revisions).FirstOrDefaultAsync(x=>x.Id==id);
    public async Task<JobDescription> AddAsync(JobDescription x){_db.JobDescriptions.Add(x);await _db.SaveChangesAsync();return x;}
    public async Task UpdateAsync(JobDescription x)=>await _db.SaveChangesAsync();
    public async Task AddBankAsync(JobDescriptionBank x){_db.JobDescriptionBanks.Add(x);await _db.SaveChangesAsync();}
    public async Task UpsertMasterAsync(JobDescriptionMaster x){var old=await _db.JobDescriptionMasters.FirstOrDefaultAsync(y=>y.Code==x.Code);if(old is null)_db.JobDescriptionMasters.Add(x);else{old.JobTitle=x.JobTitle;old.SnapshotJson=x.SnapshotJson;old.FinalizedAt=x.FinalizedAt;old.JobHolderEmployeeId=x.JobHolderEmployeeId;old.ImmediateManagerEmployeeId=x.ImmediateManagerEmployeeId;}await _db.SaveChangesAsync();}
}
