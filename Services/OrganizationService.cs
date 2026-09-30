using HRIS.Api.DTOs.Organization;
using HRIS.Api.Exceptions;
using HRIS.Api.Models;
using HRIS.Api.Repositories;

namespace HRIS.Api.Services;

public class OrganizationService : IOrganizationService
{
    private readonly IOrganizationRepository _repository;
    private readonly ILogger<OrganizationService> _logger;

    public OrganizationService(IOrganizationRepository repository, ILogger<OrganizationService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<List<OrganizationResponseDto>> GetAllAsync()
    {
        var organizations = await _repository.GetAllAsync();
        var counts = await _repository.GetEmployeeCountsAsync();
        return organizations
            .Select(o => ToDto(o, counts.TryGetValue(o.Id, out var c) ? c : 0))
            .ToList();
    }

    public async Task<OrganizationResponseDto> GetByIdAsync(int id)
    {
        var organization = await _repository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Organization dengan id {id} tidak ditemukan.");
        var counts = await _repository.GetEmployeeCountsAsync();
        return ToDto(organization, counts.TryGetValue(id, out var c) ? c : 0);
    }

    public async Task<OrganizationResponseDto> CreateAsync(OrganizationCreateDto dto)
    {
        Organization? parent = null;
        if (dto.ParentId is not null)
        {
            parent = await _repository.GetByIdAsync(dto.ParentId.Value)
                ?? throw new BadRequestException($"ParentId {dto.ParentId} tidak ditemukan.");
        }

        var organization = new Organization
        {
            OrganizationName = dto.OrganizationName,
            ParentId = dto.ParentId,
            OrganizationLevel = (parent?.OrganizationLevel ?? 0) + 1
        };

        // Path butuh Id, jadi disimpan dua kali: insert dulu, lalu isi Path.
        await _repository.AddAsync(organization);
        organization.Path = $"{parent?.Path ?? "/"}{organization.Id}/";
        await _repository.UpdateAsync(organization);

        _logger.LogInformation("Organization {Name} dibuat dengan id {Id}", organization.OrganizationName, organization.Id);
        return ToDto(organization, 0);
    }

    public async Task<OrganizationResponseDto> UpdateAsync(int id, OrganizationCreateDto dto)
    {
        var organization = await _repository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Organization dengan id {id} tidak ditemukan.");

        var oldPath = organization.Path;
        var oldLevel = organization.OrganizationLevel;

        Organization? parent = null;
        if (dto.ParentId is not null)
        {
            if (dto.ParentId == id)
                throw new BadRequestException("Organization tidak bisa menjadi induk untuk dirinya sendiri.");

            parent = await _repository.GetByIdAsync(dto.ParentId.Value)
                ?? throw new BadRequestException($"ParentId {dto.ParentId} tidak ditemukan.");

            // Cegah siklus: induk baru tidak boleh berada di bawah organisasi ini.
            if (parent.Path.StartsWith(oldPath))
                throw new BadRequestException("Induk yang dipilih berada di bawah organisasi ini, akan membuat siklus.");
        }

        organization.OrganizationName = dto.OrganizationName;
        organization.ParentId = dto.ParentId;
        organization.OrganizationLevel = (parent?.OrganizationLevel ?? 0) + 1;
        organization.Path = $"{parent?.Path ?? "/"}{organization.Id}/";
        await _repository.UpdateAsync(organization);

        // Kalau induk pindah, semua turunannya ikut berpindah path dan level.
        if (organization.Path != oldPath)
        {
            var descendants = await _repository.GetDescendantsAsync(oldPath);
            var levelShift = organization.OrganizationLevel - oldLevel;
            foreach (var d in descendants)
            {
                d.Path = organization.Path + d.Path.Substring(oldPath.Length);
                d.OrganizationLevel += levelShift;
            }
            await _repository.UpdateRangeAsync(descendants);
        }

        return await GetByIdAsync(id);
    }

    public async Task DeleteAsync(int id)
    {
        var organization = await _repository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Organization dengan id {id} tidak ditemukan.");

        if (organization.Children.Any())
            throw new BadRequestException("Organization masih punya organisasi di bawahnya, hapus atau pindahkan dulu.");

        if (await _repository.HasEmploymentsAsync(id))
            throw new BadRequestException("Organization masih dipakai data pekerjaan karyawan (termasuk riwayat), tidak bisa dihapus.");

        await _repository.DeleteAsync(organization);
    }

    private static OrganizationResponseDto ToDto(Organization o, int employeeCount) => new()
    {
        Id = o.Id,
        OrganizationName = o.OrganizationName,
        ParentId = o.ParentId,
        ParentName = o.Parent?.OrganizationName,
        OrganizationLevel = o.OrganizationLevel,
        Path = o.Path,
        IsActive = o.IsActive,
        EmployeeCount = employeeCount
    };
}
