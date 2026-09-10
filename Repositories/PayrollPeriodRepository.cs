using HRIS.Api.Data;
using HRIS.Api.Models;
using HRIS.Api.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace HRIS.Api.Repositories;

public class PayrollPeriodRepository : IPayrollPeriodRepository
{
    private readonly AppDbContext _context;

    public PayrollPeriodRepository(AppDbContext context)
    {
        _context = context;
    }

    private IQueryable<PayrollPeriod> BaseQuery() =>
        _context.PayrollPeriods
            .Include(p => p.Items)
                .ThenInclude(i => i.Employee)
            .Include(p => p.Approvals)
                .ThenInclude(a => a.Approver);

    public async Task<List<PayrollPeriod>> GetAllAsync() =>
        await BaseQuery()
            .OrderByDescending(p => p.Year)
            .ThenByDescending(p => p.Month)
            .ToListAsync();

    public async Task<PayrollPeriod?> GetByIdAsync(int id) =>
        await BaseQuery().FirstOrDefaultAsync(p => p.Id == id);

    public async Task<PayrollPeriod?> GetByMonthYearAsync(int month, int year) =>
        await BaseQuery().FirstOrDefaultAsync(p => p.Month == month && p.Year == year);

    public async Task<List<PayrollPeriod>> GetPendingForApproverAsync(int approverEmployeeId) =>
        await BaseQuery()
            .Where(p => p.Status == PayrollPeriodStatus.InApproval &&
                        p.Approvals.Any(a => a.ApproverId == approverEmployeeId &&
                                              a.Level == p.CurrentLevel &&
                                              a.Status == ApprovalStatus.Pending))
            .OrderBy(p => p.Year).ThenBy(p => p.Month)
            .ToListAsync();

    public async Task<PayrollPeriod> AddAsync(PayrollPeriod period)
    {
        _context.PayrollPeriods.Add(period);
        await _context.SaveChangesAsync();
        return period;
    }

    public async Task UpdateAsync(PayrollPeriod period)
    {
        _context.PayrollPeriods.Update(period);
        await _context.SaveChangesAsync();
    }
}
