using HRIS.Api.Data;
using HRIS.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HRIS.Api.Repositories;

public class RegulationRepository : IRegulationRepository
{
    private readonly AppDbContext _context;
    public RegulationRepository(AppDbContext context) => _context = context;

    public Task<List<Regulation>> GetAllAsync(bool includeInactive = false) => _context.Regulations
        .Where(x => includeInactive || x.IsActive)
        .OrderBy(x => x.SortOrder).ThenBy(x => x.Title)
        .ToListAsync();

    public Task<Regulation?> GetByIdAsync(int id) => _context.Regulations.FirstOrDefaultAsync(x => x.Id == id);

    public async Task<Regulation> AddAsync(Regulation regulation)
    {
        _context.Regulations.Add(regulation);
        await _context.SaveChangesAsync();
        return regulation;
    }

    public async Task UpdateAsync(Regulation regulation) => await _context.SaveChangesAsync();
}
