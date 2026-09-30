using HRIS.Api.Models;

namespace HRIS.Api.Repositories;

public record ReferenceOption(int Id, string Name);

public interface IReferenceRepository
{
    // Cek apakah baris dengan Id tertentu ada di tabel T (dipakai validasi FK di Service).
    Task<bool> ExistsAsync<T>(int id) where T : class;

    Task<EmploymentType?> GetEmploymentTypeAsync(int id);
    Task<int?> GetHierarchyTypeIdAsync(string name);
    Task<int?> GetContactTypeIdAsync(string name);
    Task<bool> IsModuleAllowedAsync(int employmentTypeId, string moduleCode);

    // Daftar Id + Nama untuk dropdown. Null kalau nama tabel referensi tidak dikenal.
    Task<List<ReferenceOption>?> GetOptionsAsync(string type);
}
