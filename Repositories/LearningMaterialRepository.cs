using HRIS.Api.Data;
using HRIS.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HRIS.Api.Repositories;

public class LearningMaterialRepository : ILearningMaterialRepository
{
    private readonly AppDbContext _context;
    public LearningMaterialRepository(AppDbContext context) => _context = context;

    private IQueryable<LearningMaterial> BaseQuery() => _context.LearningMaterials
        .Include(x => x.Document)
        .Where(x => x.IsActive);

    public Task<List<LearningMaterial>> GetAllAsync() => BaseQuery()
        .OrderByDescending(x => x.CreatedAt).ThenBy(x => x.Title).ToListAsync();

    public Task<LearningMaterial?> GetByIdAsync(int id) => BaseQuery().FirstOrDefaultAsync(x => x.Id == id);

    public async Task<LearningMaterial> AddAsync(LearningMaterial material)
    {
        _context.LearningMaterials.Add(material);
        await _context.SaveChangesAsync();
        return material;
    }

    public async Task UpdateAsync(LearningMaterial material) => await _context.SaveChangesAsync();
}
