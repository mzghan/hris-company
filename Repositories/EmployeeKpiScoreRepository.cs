using HRIS.Api.Common;
using HRIS.Api.Data;
using HRIS.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HRIS.Api.Repositories;

public class EmployeeKpiScoreRepository : IEmployeeKpiScoreRepository
{
    private readonly AppDbContext _context;

    public EmployeeKpiScoreRepository(AppDbContext context)
    {
        _context = context;
    }

    private IQueryable<EmployeeKpiScore> BaseQuery() =>
        _context.EmployeeKpiScores
            .Include(s => s.KpiPeriod)
            .Include(s => s.Employee)
            .Include(s => s.Criteria)
            .Include(s => s.FilledByUser)
            .Include(s => s.Revisions)
                .ThenInclude(r => r.RevisedByUser);

    public async Task<EmployeeKpiScore?> GetByIdAsync(int id) =>
        await BaseQuery().FirstOrDefaultAsync(s => s.Id == id);

    public async Task<List<EmployeeKpiScore>> GetByPeriodAsync(int kpiPeriodId) =>
        await BaseQuery()
            .Where(s => s.KpiPeriodId == kpiPeriodId)
            .OrderBy(s => s.EmployeeId).ThenBy(s => s.CriteriaId)
            .ToListAsync();

    public async Task<List<EmployeeKpiScore>> GetByPeriodAndEmployeeAsync(int kpiPeriodId, int employeeId) =>
        await BaseQuery()
            .Where(s => s.KpiPeriodId == kpiPeriodId && s.EmployeeId == employeeId)
            .OrderBy(s => s.CriteriaId)
            .ToListAsync();

    public async Task<EmployeeKpiScore?> GetByPeriodEmployeeCriteriaAsync(int kpiPeriodId, int employeeId, int criteriaId) =>
        await BaseQuery()
            .FirstOrDefaultAsync(s => s.KpiPeriodId == kpiPeriodId
                                    && s.EmployeeId == employeeId
                                    && s.CriteriaId == criteriaId);

    public async Task<List<EmployeeKpiScore>> GetByPeriodForSubordinatesAsync(int kpiPeriodId, int managerEmployeeId) =>
        await BaseQuery()
            .Where(s => s.KpiPeriodId == kpiPeriodId && s.Employee!.Managers.Any(h => h.ManagerId == managerEmployeeId && h.EndDate == null && h.HierarchyType!.HierarchyTypeName == RefNames.DirectManager))
            .OrderBy(s => s.EmployeeId).ThenBy(s => s.CriteriaId)
            .ToListAsync();

    public async Task<EmployeeKpiScore> AddAsync(EmployeeKpiScore score)
    {
        _context.EmployeeKpiScores.Add(score);
        await _context.SaveChangesAsync();
        return score;
    }

    public async Task UpdateAsync(EmployeeKpiScore score)
    {
        _context.EmployeeKpiScores.Update(score);
        await _context.SaveChangesAsync();
    }

    public async Task OverrideAsync(EmployeeKpiScore score, KpiScoreRevision revision)
    {
        _context.KpiScoreRevisions.Add(revision);
        _context.EmployeeKpiScores.Update(score);
        await _context.SaveChangesAsync();
    }
}
