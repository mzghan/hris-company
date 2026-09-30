using HRIS.Api.Common;
using HRIS.Api.DTOs.Expatriate;
using HRIS.Api.Exceptions;
using HRIS.Api.Models;
using HRIS.Api.Repositories;

namespace HRIS.Api.Services;

public class ExpatriateService : IExpatriateService
{
    private readonly IExpatriateRepository _repository;
    private readonly IReferenceRepository _referenceRepository;
    private readonly IAuditLogService _auditLogService;

    public ExpatriateService(IExpatriateRepository repository, IReferenceRepository referenceRepository, IAuditLogService auditLogService)
    {
        _repository = repository; _referenceRepository = referenceRepository; _auditLogService = auditLogService;
    }

    public async Task<List<ExpatriateEmployeeResponseDto>> GetExpatriatesAsync(UserContext actor)
    {
        EnsureHrOrSupport(actor);
        return (await _repository.GetExpatriatesAsync()).Select(ToDto).ToList();
    }

    public async Task<ExpatriateEmployeeResponseDto> GetAsync(int employeeId, UserContext actor)
    {
        EnsureOwnerOrHr(actor, employeeId);
        var employee = await _repository.GetEmployeeAsync(employeeId) ?? throw new NotFoundException("Karyawan ekspatriat tidak ditemukan.");
        return ToDto(employee);
    }

    public async Task<EmployeeIdentityResponseDto> AddIdentityAsync(int employeeId, EmployeeIdentityCreateDto dto, UserContext actor)
    {
        EnsureOwnerOrHr(actor, employeeId);
        var employee = await _repository.GetEmployeeAsync(employeeId) ?? throw new NotFoundException("Karyawan ekspatriat tidak ditemukan.");
        await ValidateIdentityAsync(dto, employeeId);
        if (dto.IsPrimary)
        {
            foreach (var old in employee.Identities) old.IsPrimary = false;
        }
        var identity = await _repository.AddIdentityAsync(new EmployeeIdentity
        {
            EmployeeId = employeeId, IdentityTypeId = dto.IdentityTypeId,
            IdentityNumber = dto.IdentityNumber.Trim(), ValidUntil = dto.ValidUntil, IsPrimary = dto.IsPrimary
        });
        if (actor.IsSupport) await _auditLogService.LogAsync(actor.UserId, "Expatriate.IdentityCreate", nameof(EmployeeIdentity), identity.Id, employee.FullName);
        return ToDto((await _repository.GetIdentityAsync(identity.Id))!);
    }

    public async Task<EmployeeIdentityResponseDto> UpdateIdentityAsync(int id, EmployeeIdentityCreateDto dto, UserContext actor)
    {
        var identity = await _repository.GetIdentityAsync(id) ?? throw new NotFoundException("Identitas tidak ditemukan.");
        EnsureOwnerOrHr(actor, identity.EmployeeId);
        await ValidateIdentityAsync(dto, identity.EmployeeId, id);
        var employee = await _repository.GetEmployeeAsync(identity.EmployeeId) ?? throw new NotFoundException("Karyawan ekspatriat tidak ditemukan.");
        if (dto.IsPrimary)
        {
            foreach (var old in employee.Identities.Where(x => x.Id != id)) old.IsPrimary = false;
        }
        identity.IdentityTypeId = dto.IdentityTypeId; identity.IdentityNumber = dto.IdentityNumber.Trim();
        identity.ValidUntil = dto.ValidUntil; identity.IsPrimary = dto.IsPrimary;
        await _repository.UpdateIdentityAsync(identity);
        if (actor.IsSupport) await _auditLogService.LogAsync(actor.UserId, "Expatriate.IdentityUpdate", nameof(EmployeeIdentity), id, employee.FullName);
        return ToDto((await _repository.GetIdentityAsync(id))!);
    }

    public async Task DeleteIdentityAsync(int id, UserContext actor)
    {
        var identity = await _repository.GetIdentityAsync(id) ?? throw new NotFoundException("Identitas tidak ditemukan.");
        EnsureOwnerOrHr(actor, identity.EmployeeId);
        await _repository.DeleteIdentityAsync(identity);
        if (actor.IsSupport) await _auditLogService.LogAsync(actor.UserId, "Expatriate.IdentityDelete", nameof(EmployeeIdentity), id, identity.Employee?.FullName);
    }

    private async Task ValidateIdentityAsync(EmployeeIdentityCreateDto dto, int employeeId, int? exceptId = null)
    {
        if (string.IsNullOrWhiteSpace(dto.IdentityNumber)) throw new BadRequestException("Nomor identitas wajib diisi.");
        if (!await _referenceRepository.ExistsAsync<IdentityType>(dto.IdentityTypeId)) throw new BadRequestException("Tipe identitas tidak ditemukan.");
        if (await _repository.IdentityNumberExistsAsync(employeeId, dto.IdentityTypeId, dto.IdentityNumber.Trim(), exceptId))
            throw new BadRequestException("Nomor identitas dengan tipe tersebut sudah terdaftar untuk karyawan ini.");
    }

    private static void EnsureHrOrSupport(UserContext actor)
    {
        if (!actor.IsHrOrSupport) throw new ForbiddenException("Hanya HR/Support yang boleh melihat daftar ekspatriat.");
    }

    private static void EnsureOwnerOrHr(UserContext actor, int employeeId)
    {
        if (actor.IsHrOrSupport) return;
        if (actor.EmployeeId != employeeId) throw new ForbiddenException("Kamu tidak punya akses ke data ekspatriat ini.");
    }

    private static ExpatriateEmployeeResponseDto ToDto(Employee e) => new()
    {
        EmployeeId = e.Id, EmployeeNumber = e.EmployeeNumber, FullName = e.FullName,
        Nationality = e.NationalityCountry?.CountryName ?? "-",
        WorkEmail = e.Contacts.FirstOrDefault(c => c.ContactType?.ContactTypeName == "Work Email")?.ContactValue,
        JoinDate = e.JoinDate, Identities = e.Identities.OrderBy(i => i.IdentityType?.IdentityTypeName).Select(ToDto).ToList()
    };

    private static EmployeeIdentityResponseDto ToDto(EmployeeIdentity i) => new()
    {
        Id = i.Id, EmployeeId = i.EmployeeId, EmployeeName = i.Employee?.FullName ?? string.Empty,
        IdentityTypeId = i.IdentityTypeId, IdentityTypeName = i.IdentityType?.IdentityTypeName ?? string.Empty,
        IdentityNumber = i.IdentityNumber, ValidUntil = i.ValidUntil, IsPrimary = i.IsPrimary
    };
}
