using HRIS.Api.DTOs.Payroll;
using HRIS.Api.Exceptions;
using HRIS.Api.Models;
using HRIS.Api.Models.Enums;
using HRIS.Api.Repositories;

namespace HRIS.Api.Services;

public class PayrollService : IPayrollService
{
    private readonly IPayrollPeriodRepository _periodRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IEmployeeSalaryRepository _salaryRepository;
    private readonly ILogger<PayrollService> _logger;
    private readonly List<int> _approverEmployeeIds;

    public PayrollService(
        IPayrollPeriodRepository periodRepository,
        IEmployeeRepository employeeRepository,
        IEmployeeSalaryRepository salaryRepository,
        ILogger<PayrollService> logger,
        IConfiguration config)
    {
        _periodRepository = periodRepository;
        _employeeRepository = employeeRepository;
        _salaryRepository = salaryRepository;
        _logger = logger;

        // Berbeda dengan LeaveApproval (rantai approver ditentukan dari
        // Employee.ManagerId per-employee), PayrollPeriod bukan milik satu
        // employee, jadi tidak ada "manager chain" alami untuk diikuti.
        // Untuk MVP, urutan approver level 1..N ditentukan lewat konfigurasi
        // (mis. Manager dulu baru Director/Admin). Ini salah satu poin
        // terbuka di bab 9.4 technical alignment doc — didiskusikan lagi
        // kalau butuh jadi lebih dinamis (mis. per-departemen).
        var raw = config["PayrollApproval:ApproverEmployeeIds"] ?? string.Empty;
        _approverEmployeeIds = raw
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(int.Parse)
            .ToList();
    }

    public async Task<PayrollPeriodResponseDto> CreatePeriodAsync(PayrollPeriodCreateDto dto)
    {
        var existing = await _periodRepository.GetByMonthYearAsync(dto.Month, dto.Year);
        if (existing is not null)
            throw new BadRequestException($"PayrollPeriod untuk {dto.Month}/{dto.Year} sudah ada.");

        if (_approverEmployeeIds.Count == 0)
            throw new BadRequestException("PayrollApproval:ApproverEmployeeIds belum dikonfigurasi di appsettings.");

        var referenceDate = new DateOnly(dto.Year, dto.Month, 1);
        var activeEmployees = await _employeeRepository.GetActiveEmployeesAsync();

        var period = new PayrollPeriod
        {
            Month = dto.Month,
            Year = dto.Year,
            Status = PayrollPeriodStatus.InApproval,
            CurrentLevel = 1
        };

        foreach (var employee in activeEmployees)
        {
            var salary = await _salaryRepository.GetActiveAsync(employee.Id, referenceDate);
            if (salary is null)
            {
                // Employee aktif tapi belum punya data EmployeeSalary yang
                // berlaku pada periode ini — dilewati, bukan menggagalkan
                // seluruh pembuatan periode. Perlu direview manual lewat
                // GET /api/payrollperiods/{id} (employee ybs tidak akan
                // muncul di daftar Items).
                _logger.LogWarning(
                    "Employee {EmployeeId} tidak punya EmployeeSalary aktif per {ReferenceDate}, dilewati dari PayrollPeriod {Month}/{Year}",
                    employee.Id, referenceDate, dto.Month, dto.Year);
                continue;
            }

            var grossPay = salary.BaseSalary + salary.AllowanceTotal;
            const decimal totalDeduction = 0; // rumus potongan belum difinalkan, lihat bab 9.4

            period.Items.Add(new PayrollItem
            {
                EmployeeId = employee.Id,
                BaseSalary = salary.BaseSalary,
                TotalAllowance = salary.AllowanceTotal,
                TotalDeduction = totalDeduction,
                GrossPay = grossPay,
                NetPay = grossPay - totalDeduction
            });
        }

        if (period.Items.Count == 0)
            throw new BadRequestException("Tidak ada employee aktif dengan data gaji yang berlaku untuk periode ini.");

        for (int i = 0; i < _approverEmployeeIds.Count; i++)
        {
            period.Approvals.Add(new PayrollApproval
            {
                ApproverId = _approverEmployeeIds[i],
                Level = i + 1,
                Status = ApprovalStatus.Pending
            });
        }

        await _periodRepository.AddAsync(period);
        _logger.LogInformation(
            "PayrollPeriod {Month}/{Year} dibuat dengan {ItemCount} item dan {LevelCount} level approval",
            dto.Month, dto.Year, period.Items.Count, _approverEmployeeIds.Count);

        var created = await _periodRepository.GetByIdAsync(period.Id);
        return ToDto(created!);
    }

    public async Task<List<PayrollPeriodResponseDto>> GetAllAsync()
    {
        var periods = await _periodRepository.GetAllAsync();
        return periods.Select(ToDto).ToList();
    }

    public async Task<PayrollPeriodResponseDto> GetByIdAsync(int id)
    {
        var period = await _periodRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"PayrollPeriod dengan id {id} tidak ditemukan.");
        return ToDto(period);
    }

    public async Task<List<PayrollPeriodResponseDto>> GetPendingForApproverAsync(int approverEmployeeId)
    {
        var periods = await _periodRepository.GetPendingForApproverAsync(approverEmployeeId);
        return periods.Select(ToDto).ToList();
    }

    public async Task<PayrollPeriodResponseDto> ApproveAsync(int periodId, int approverEmployeeId, PayrollApprovalActionDto dto)
    {
        var (period, currentApproval) = await GetActiveApprovalOrThrow(periodId, approverEmployeeId);

        currentApproval.Status = ApprovalStatus.Approved;
        currentApproval.Note = dto.Note;
        currentApproval.ActedAt = DateTime.UtcNow;

        var nextLevel = period.Approvals.FirstOrDefault(a => a.Level == period.CurrentLevel + 1);
        if (nextLevel is not null)
        {
            period.CurrentLevel += 1;
        }
        else
        {
            period.Status = PayrollPeriodStatus.Approved;
        }

        await _periodRepository.UpdateAsync(period);
        _logger.LogInformation(
            "PayrollPeriod {Id} di-approve oleh employee {ApproverId} di level {Level}",
            periodId, approverEmployeeId, currentApproval.Level);

        var updated = await _periodRepository.GetByIdAsync(periodId);
        return ToDto(updated!);
    }

    public async Task<PayrollPeriodResponseDto> RejectAsync(int periodId, int approverEmployeeId, PayrollApprovalActionDto dto)
    {
        var (period, currentApproval) = await GetActiveApprovalOrThrow(periodId, approverEmployeeId);

        currentApproval.Status = ApprovalStatus.Rejected;
        currentApproval.Note = dto.Note;
        currentApproval.ActedAt = DateTime.UtcNow;
        period.Status = PayrollPeriodStatus.Rejected;

        await _periodRepository.UpdateAsync(period);
        _logger.LogInformation(
            "PayrollPeriod {Id} di-reject oleh employee {ApproverId} di level {Level}",
            periodId, approverEmployeeId, currentApproval.Level);

        var updated = await _periodRepository.GetByIdAsync(periodId);
        return ToDto(updated!);
    }

    public async Task<PayrollPeriodResponseDto> MarkPaidAsync(int periodId)
    {
        var period = await _periodRepository.GetByIdAsync(periodId)
            ?? throw new NotFoundException($"PayrollPeriod dengan id {periodId} tidak ditemukan.");

        if (period.Status != PayrollPeriodStatus.Approved)
            throw new BadRequestException("PayrollPeriod hanya bisa ditandai Paid setelah statusnya Approved.");

        period.Status = PayrollPeriodStatus.Paid;
        await _periodRepository.UpdateAsync(period);
        _logger.LogInformation("PayrollPeriod {Id} ditandai Paid", periodId);

        var updated = await _periodRepository.GetByIdAsync(periodId);
        return ToDto(updated!);
    }

    private async Task<(PayrollPeriod period, PayrollApproval approval)> GetActiveApprovalOrThrow(int periodId, int approverEmployeeId)
    {
        var period = await _periodRepository.GetByIdAsync(periodId)
            ?? throw new NotFoundException($"PayrollPeriod dengan id {periodId} tidak ditemukan.");

        if (period.Status != PayrollPeriodStatus.InApproval)
            throw new BadRequestException("PayrollPeriod ini sudah tidak berstatus InApproval.");

        var approval = period.Approvals.FirstOrDefault(a => a.Level == period.CurrentLevel)
            ?? throw new BadRequestException("Tidak ada approval aktif pada level ini.");

        if (approval.ApproverId != approverEmployeeId)
            throw new ForbiddenException("Anda bukan approver untuk level yang sedang aktif pada payroll period ini.");

        return (period, approval);
    }

    private static PayrollPeriodResponseDto ToDto(PayrollPeriod p) => new()
    {
        Id = p.Id,
        Month = p.Month,
        Year = p.Year,
        Status = p.Status.ToString(),
        CurrentLevel = p.CurrentLevel,
        CreatedAt = p.CreatedAt,
        TotalNetPay = p.Items.Sum(i => i.NetPay),
        Items = p.Items.Select(i => new PayrollItemResponseDto
        {
            Id = i.Id,
            EmployeeId = i.EmployeeId,
            EmployeeName = i.Employee?.FullName ?? string.Empty,
            BaseSalary = i.BaseSalary,
            TotalAllowance = i.TotalAllowance,
            TotalDeduction = i.TotalDeduction,
            GrossPay = i.GrossPay,
            NetPay = i.NetPay
        }).ToList(),
        Approvals = p.Approvals
            .OrderBy(a => a.Level)
            .Select(a => new PayrollApprovalResponseDto
            {
                Id = a.Id,
                Level = a.Level,
                Status = a.Status.ToString(),
                ApproverId = a.ApproverId,
                ApproverName = a.Approver?.FullName ?? string.Empty,
                Note = a.Note,
                ActedAt = a.ActedAt
            }).ToList()
    };
}
