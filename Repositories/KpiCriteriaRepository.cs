using HRIS.Api.Data;
using HRIS.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HRIS.Api.Repositories;

public class KpiCriteriaRepository : IKpiCriteriaRepository
{
    private readonly AppDbContext _context;

    public KpiCriteriaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<KpiCriteria>> GetAllAsync() =>
        await _context.KpiCriteria.OrderBy(c => c.Name).ToListAsync();

    public async Task<KpiCriteria?> GetByIdAsync(int id) =>
        await _context.KpiCriteria.FirstOrDefaultAsync(c => c.Id == id);

    public async Task<KpiCriteria> AddAsync(KpiCriteria criteria)
    {
        _context.KpiCriteria.Add(criteria);
        await _context.SaveChangesAsync();
        return criteria;
    }

    public async Task UpdateAsync(KpiCriteria criteria)
    {
        _context.KpiCriteria.Update(criteria);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(KpiCriteria criteria)
    {
        _context.KpiCriteria.Remove(criteria);
        await _context.SaveChangesAsync();
    }
}
