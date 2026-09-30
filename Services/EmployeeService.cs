using HRIS.Api.Common;
using HRIS.Api.DTOs.Employee;
using HRIS.Api.Exceptions;
using HRIS.Api.Models;
using HRIS.Api.Repositories;

namespace HRIS.Api.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _repository;
    private readonly IReferenceRepository _referenceRepository;
    private readonly ILogger<EmployeeService> _logger;

    public EmployeeService(
        IEmployeeRepository repository,
        IReferenceRepository referenceRepository,
        ILogger<EmployeeService> logger)
    {
        _repository = repository;
        _referenceRepository = referenceRepository;
        _logger = logger;
    }

    public async Task<List<EmployeeResponseDto>> GetAllAsync()
    {
        var employees = await _repository.GetAllAsync();
        return employees.Select(ToDto).ToList();
    }

    public async Task<EmployeeResponseDto> GetByIdAsync(int id)
    {
        var employee = await _repository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Employee dengan id {id} tidak ditemukan.");
        return ToDto(employee);
    }

    public async Task<EmployeeResponseDto> CreateAsync(EmployeeCreateDto dto)
    {
        if (await _repository.GetByNumberAsync(dto.EmployeeNumber) is not null)
            throw new BadRequestException($"Nomor karyawan {dto.EmployeeNumber} sudah dipakai employee lain.");

        if (await _repository.WorkEmailExistsAsync(dto.WorkEmail))
            throw new BadRequestException($"Email {dto.WorkEmail} sudah dipakai employee lain.");

        await ValidatePersonalReferencesAsync(dto.NationalityCountryId, dto.ReligionId, dto.GenderId, dto.MaritalStatusId);
        await ValidateEmploymentAsync(
            dto.EmploymentTypeId, dto.EmploymentStatusId, dto.VendorId, dto.OrganizationId,
            dto.LocationId, dto.JobLevelId, dto.JobTitleId, dto.GradeId);

        var workEmailTypeId = await _referenceRepository.GetContactTypeIdAsync(RefNames.WorkEmail)
            ?? throw new BadRequestException($"REF_Contact_Type belum berisi '{RefNames.WorkEmail}'. Jalankan seeder.");

        var employee = new Employee
        {
            EmployeeNumber = dto.EmployeeNumber,
            FullName = dto.FullName,
            BirthDate = dto.BirthDate,
            JoinDate = dto.JoinDate,
            NationalityCountryId = dto.NationalityCountryId,
            ReligionId = dto.ReligionId,
            GenderId = dto.GenderId,
            MaritalStatusId = dto.MaritalStatusId
        };

        employee.Contacts.Add(new EmployeeContact
        {
            ContactTypeId = workEmailTypeId,
            ContactValue = dto.WorkEmail,
            IsPrimary = true
        });

        employee.Employments.Add(new EmployeeEmployment
        {
            EmploymentTypeId = dto.EmploymentTypeId,
            EmploymentStatusId = dto.EmploymentStatusId,
            VendorId = dto.VendorId,
            OrganizationId = dto.OrganizationId,
            LocationId = dto.LocationId,
            JobLevelId = dto.JobLevelId,
            JobTitleId = dto.JobTitleId,
            GradeId = dto.GradeId,
            IsFte = dto.IsFte,
            IsSales = dto.IsSales,
            ContractEndDate = dto.ContractEndDate,
            StartDate = dto.JoinDate
        });

        if (dto.DirectManagerId is not null)
        {
            if (!await _referenceRepository.ExistsAsync<Employee>(dto.DirectManagerId.Value))
                throw new BadRequestException($"DirectManagerId {dto.DirectManagerId} tidak ditemukan.");

            var directTypeId = await GetDirectManagerTypeIdAsync();
            employee.Managers.Add(new EmployeeHierarchy
            {
                ManagerId = dto.DirectManagerId.Value,
                HierarchyTypeId = directTypeId,
                StartDate = dto.JoinDate
            });
        }

        await _repository.AddAsync(employee);
        _logger.LogInformation("Employee {Name} dibuat dengan id {Id}", employee.FullName, employee.Id);

        var created = await _repository.GetByIdAsync(employee.Id);
        return ToDto(created!);
    }

    public async Task<EmployeeResponseDto> UpdateAsync(int id, EmployeeUpdateDto dto)
    {
        var employee = await _repository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Employee dengan id {id} tidak ditemukan.");

        await ValidatePersonalReferencesAsync(dto.NationalityCountryId, dto.ReligionId, dto.GenderId, dto.MaritalStatusId);

        if (!dto.IsActive && employee.IsActive && await _repository.HasActiveSubordinatesAsync(id))
            throw new BadRequestException("Employee ini masih menjadi atasan langsung karyawan aktif, pindahkan bawahannya dulu.");

        employee.FullName = dto.FullName;
        employee.BirthDate = dto.BirthDate;
        employee.JoinDate = dto.JoinDate;
        employee.NationalityCountryId = dto.NationalityCountryId;
        employee.ReligionId = dto.ReligionId;
        employee.GenderId = dto.GenderId;
        employee.MaritalStatusId = dto.MaritalStatusId;
        employee.IsActive = dto.IsActive;

        await _repository.UpdateAsync(employee);

        var updated = await _repository.GetByIdAsync(id);
        return ToDto(updated!);
    }

    public async Task<EmployeeResponseDto> ChangeEmploymentAsync(int id, EmploymentChangeDto dto)
    {
        _ = await _repository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Employee dengan id {id} tidak ditemukan.");

        var current = await _repository.GetCurrentEmploymentAsync(id);
        if (current is not null && dto.EffectiveDate <= current.StartDate)
            throw new BadRequestException("EffectiveDate harus setelah StartDate pekerjaan saat ini.");

        await ValidateEmploymentAsync(
            dto.EmploymentTypeId, dto.EmploymentStatusId, dto.VendorId, dto.OrganizationId,
            dto.LocationId, dto.JobLevelId, dto.JobTitleId, dto.GradeId);
        await EnsureExistsAsync<EndReason>(dto.EndReasonId, "EndReasonId");

        // Jangan update di tempat: tutup baris lama, lalu buat baris baru,
        // supaya riwayat jabatan/grade/organisasi tetap terjaga.
        if (current is not null)
        {
            current.EndDate = dto.EffectiveDate.AddDays(-1);
            current.EndReasonId = dto.EndReasonId;
        }

        var next = new EmployeeEmployment
        {
            EmployeeId = id,
            EmploymentTypeId = dto.EmploymentTypeId,
            EmploymentStatusId = dto.EmploymentStatusId,
            VendorId = dto.VendorId,
            OrganizationId = dto.OrganizationId,
            LocationId = dto.LocationId,
            JobLevelId = dto.JobLevelId,
            JobTitleId = dto.JobTitleId,
            GradeId = dto.GradeId,
            IsFte = dto.IsFte,
            IsSales = dto.IsSales,
            ContractEndDate = dto.ContractEndDate,
            StartDate = dto.EffectiveDate
        };

        await _repository.ReplaceEmploymentAsync(current, next);
        _logger.LogInformation("Employment employee {Id} diganti, berlaku mulai {Date}", id, dto.EffectiveDate);

        return await GetByIdAsync(id);
    }

    public async Task<EmployeeResponseDto> ChangeDirectManagerAsync(int id, ChangeManagerDto dto)
    {
        _ = await _repository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Employee dengan id {id} tidak ditemukan.");

        if (dto.ManagerId == id)
            throw new BadRequestException("Employee tidak bisa menjadi manager untuk dirinya sendiri.");

        if (dto.ManagerId is not null)
        {
            if (!await _referenceRepository.ExistsAsync<Employee>(dto.ManagerId.Value))
                throw new BadRequestException($"ManagerId {dto.ManagerId} tidak ditemukan.");

            // Cegah siklus: calon atasan tidak boleh sudah berada di bawah employee ini.
            var chain = await _repository.GetManagerChainAsync(dto.ManagerId.Value, 50);
            if (chain.Any(m => m.Id == id))
                throw new BadRequestException("Atasan yang dipilih berada di bawah employee ini, akan membuat siklus atasan.");
        }

        var current = await _repository.GetActiveDirectManagerRowAsync(id);
        if (current?.ManagerId == dto.ManagerId)
            throw new BadRequestException("Atasan langsung tidak berubah.");

        if (current is not null && dto.EffectiveDate <= current.StartDate)
            throw new BadRequestException("EffectiveDate harus setelah tanggal mulai atasan saat ini.");

        if (current is not null)
            current.EndDate = dto.EffectiveDate.AddDays(-1);

        EmployeeHierarchy? next = null;
        if (dto.ManagerId is not null)
        {
            next = new EmployeeHierarchy
            {
                EmployeeId = id,
                ManagerId = dto.ManagerId.Value,
                HierarchyTypeId = await GetDirectManagerTypeIdAsync(),
                StartDate = dto.EffectiveDate
            };
        }

        await _repository.ReplaceDirectManagerAsync(current, next);
        return await GetByIdAsync(id);
    }

    public async Task DeleteAsync(int id)
    {
        var employee = await _repository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Employee dengan id {id} tidak ditemukan.");

        if (await _repository.HasActiveSubordinatesAsync(id))
            throw new BadRequestException("Employee ini masih menjadi atasan langsung karyawan aktif, pindahkan bawahannya dulu.");

        await _repository.DeleteAsync(employee);
    }

    // ---------------- Validasi ----------------

    private async Task<int> GetDirectManagerTypeIdAsync() =>
        await _referenceRepository.GetHierarchyTypeIdAsync(RefNames.DirectManager)
            ?? throw new BadRequestException($"REF_Hierarchy_Type belum berisi '{RefNames.DirectManager}'. Jalankan seeder.");

    private async Task EnsureExistsAsync<T>(int? id, string label) where T : class
    {
        if (id is null) return;
        if (!await _referenceRepository.ExistsAsync<T>(id.Value))
            throw new BadRequestException($"{label} {id} tidak ditemukan.");
    }

    private async Task ValidatePersonalReferencesAsync(int? countryId, int? religionId, int? genderId, int? maritalStatusId)
    {
        await EnsureExistsAsync<Country>(countryId, "NationalityCountryId");
        await EnsureExistsAsync<Religion>(religionId, "ReligionId");
        await EnsureExistsAsync<Gender>(genderId, "GenderId");
        await EnsureExistsAsync<MaritalStatus>(maritalStatusId, "MaritalStatusId");
    }

    private async Task ValidateEmploymentAsync(
        int employmentTypeId, int employmentStatusId, int? vendorId, int organizationId,
        int? locationId, int jobLevelId, int jobTitleId, int? gradeId)
    {
        var employmentType = await _referenceRepository.GetEmploymentTypeAsync(employmentTypeId)
            ?? throw new BadRequestException($"EmploymentTypeId {employmentTypeId} tidak ditemukan.");

        // Aturan vendor: Outsource wajib vendor, tipe lain harus kosong.
        var isOutsource = employmentType.EmploymentTypeName.Equals(RefNames.Outsource, StringComparison.OrdinalIgnoreCase);
        if (isOutsource && vendorId is null)
            throw new BadRequestException("Vendor wajib diisi untuk karyawan Outsource.");
        if (!isOutsource && vendorId is not null)
            throw new BadRequestException("Vendor hanya boleh diisi untuk karyawan Outsource.");

        await EnsureExistsAsync<EmploymentStatus>(employmentStatusId, "EmploymentStatusId");
        await EnsureExistsAsync<Vendor>(vendorId, "VendorId");
        await EnsureExistsAsync<Organization>(organizationId, "OrganizationId");
        await EnsureExistsAsync<Location>(locationId, "LocationId");
        await EnsureExistsAsync<JobLevel>(jobLevelId, "JobLevelId");
        await EnsureExistsAsync<JobTitle>(jobTitleId, "JobTitleId");
        await EnsureExistsAsync<Grade>(gradeId, "GradeId");
    }

    // ---------------- Mapping ----------------

    private static EmployeeResponseDto ToDto(Employee e)
    {
        var employment = e.Employments.FirstOrDefault(m => m.EndDate == null);
        var directManager = e.Managers.FirstOrDefault(h =>
            h.EndDate == null && h.HierarchyType?.HierarchyTypeName == RefNames.DirectManager);
        var workEmail = e.Contacts
            .FirstOrDefault(c => c.ContactType?.ContactTypeName == RefNames.WorkEmail)?.ContactValue;

        return new EmployeeResponseDto
        {
            Id = e.Id,
            EmployeeNumber = e.EmployeeNumber,
            FullName = e.FullName,
            WorkEmail = workEmail,
            BirthDate = e.BirthDate,
            JoinDate = e.JoinDate,
            IsActive = e.IsActive,
            NationalityCountryId = e.NationalityCountryId,
            NationalityCountryName = e.NationalityCountry?.CountryName,
            ReligionId = e.ReligionId,
            GenderId = e.GenderId,
            GenderName = e.Gender?.GenderName,
            MaritalStatusId = e.MaritalStatusId,

            EmploymentId = employment?.Id,
            EmploymentTypeId = employment?.EmploymentTypeId,
            EmploymentTypeName = employment?.EmploymentType?.EmploymentTypeName,
            EmploymentStatusId = employment?.EmploymentStatusId,
            EmploymentStatusName = employment?.EmploymentStatus?.EmploymentStatusName,
            VendorId = employment?.VendorId,
            VendorName = employment?.Vendor?.VendorName,
            OrganizationId = employment?.OrganizationId,
            OrganizationName = employment?.Organization?.OrganizationName,
            LocationId = employment?.LocationId,
            LocationName = employment?.Location?.LocationName,
            JobLevelId = employment?.JobLevelId,
            JobLevelName = employment?.JobLevel?.JobLevelName,
            JobTitleId = employment?.JobTitleId,
            JobTitleName = employment?.JobTitle?.JobTitleName,
            GradeId = employment?.GradeId,
            GradeLevel = employment?.Grade?.GradeLevel,
            IsFte = employment?.IsFte,
            IsSales = employment?.IsSales,
            EmploymentStartDate = employment?.StartDate,
            ContractEndDate = employment?.ContractEndDate,

            DirectManagerId = directManager?.ManagerId,
            DirectManagerName = directManager?.Manager?.FullName
        };
    }
}
