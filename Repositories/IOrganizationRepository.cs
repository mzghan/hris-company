using HRIS.Api.Models;

namespace HRIS.Api.Repositories;

public interface IOrganizationRepository
{
    Task<List<Organization>> GetAllAsync();
    Task<Organization?> GetByIdAsync(int id);

    // Semua organisasi di bawah path tertentu (tidak termasuk dirinya sendiri).
    Task<List<Organization>> GetDescendantsAsync(string path);

    // Jumlah karyawan dengan Employment terkini di tiap organisasi (langsung, tanpa anak).
    Task<Dictionary<int, int>> GetEmployeeCountsAsync();

    Task<bool> HasEmploymentsAsync(int organizationId);
    Task<Organization> AddAsync(Organization organization);
    Task UpdateAsync(Organization organization);
    Task UpdateRangeAsync(IEnumerable<Organization> organizations);
    Task DeleteAsync(Organization organization);
}
