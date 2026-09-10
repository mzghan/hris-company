using HRIS.Api.Data;
using HRIS.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HRIS.Api.Repositories;

public class KpiPeriodRepository : IKpiPeriodRepository
{
    private readonly AppDbContext _context;

    public KpiPeriodRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<KpiPeriod>> GetAllAsync() =>
        await _context.KpiPeriods
            .OrderByDescending(p => p.Year)
            .ThenByDescending(p => p.Id)
            .ToListAsync();

    public async Task<KpiPeriod?> GetByIdAsync(int id) =>
        await _context.KpiPeriods.FirstOrDefaultAsync(p => p.Id == id);

    public async Task<KpiPeriod?> GetByNameYearAsync(string name, int year) =>
        await _context.KpiPeriods.FirstOrDefaultAsync(p => p.Name == name && p.Year == year);

    public async Task<KpiPeriod> AddAsync(KpiPeriod period)
    {
        _context.KpiPeriods.Add(period);
        await _context.SaveChangesAsync();
        return period;
    }

    public async Task UpdateAsync(KpiPeriod period)
    {
        _context.KpiPeriods.Update(period);
        await _context.SaveChangesAsync();
    }
}
