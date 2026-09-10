using HRIS.Api.DTOs.Kpi;
using HRIS.Api.Exceptions;
using HRIS.Api.Models;
using HRIS.Api.Models.Enums;
using HRIS.Api.Repositories;

namespace HRIS.Api.Services;

// Berbeda dari LeaveRequestService/PayrollService: di sini tidak ada
// approval berjenjang (CurrentLevel, chain of approvers). Alurnya
// Admin isi nilai -> Manager boleh override dengan Note wajib -> setiap
// override tercatat sebagai KpiScoreRevision, bukan mengganti Status
// baris per level seperti LeaveApproval/PayrollApproval.
public class KpiService : IKpiService
{
    private readonly IKpiCriteriaRepository _criteriaRepository;
    private readonly IKpiPeriodRepository _periodRepository;
    private readonly IEmployeeKpiScoreRepository _scoreRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ILogger<KpiService> _logger;

    public KpiService(
        IKpiCriteriaRepository criteriaRepository,
        IKpiPeriodRepository periodRepository,
        IEmployeeKpiScoreRepository scoreRepository,
        IEmployeeRepository employeeRepository,
        ILogger<KpiService> logger)
    {
        _criteriaRepository = criteriaRepository;
        _periodRepository = periodRepository;
        _scoreRepository = scoreRepository;
        _employeeRepository = employeeRepository;
        _logger = logger;
    }

    // ================= KpiCriteria =================

    public async Task<KpiCriteriaResponseDto> CreateCriteriaAsync(KpiCriteriaCreateDto dto)
    {
        var criteria = new KpiCriteria { Name = dto.Name, Weight = dto.Weight };
        await _criteriaRepository.AddAsync(criteria);
        _logger.LogInformation("KpiCriteria '{Name}' dibuat dengan Weight {Weight}", criteria.Name, criteria.Weight);
        return ToDto(criteria);
    }

    public async Task<List<KpiCriteriaResponseDto>> GetAllCriteriaAsync()
    {
        var criteria = await _criteriaRepository.GetAllAsync();
        return criteria.Select(ToDto).ToList();
    }

    public async Task<KpiCriteriaResponseDto> UpdateCriteriaAsync(int id, KpiCriteriaUpdateDto dto)
    {
        var criteria = await _criteriaRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"KpiCriteria dengan id {id} tidak ditemukan.");

        criteria.Name = dto.Name;
        criteria.Weight = dto.Weight;
        await _criteriaRepository.UpdateAsync(criteria);
        _logger.LogInformation("KpiCriteria {Id} diupdate", id);

        return ToDto(criteria);
    }

    public async Task DeleteCriteriaAsync(int id)
    {
        var criteria = await _criteriaRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"KpiCriteria dengan id {id} tidak ditemukan.");

        // Kalau kriteria ini sudah dipakai di EmployeeKpiScore, FK Restrict
        // di AppDbContext akan menolak delete-nya (muncul sebagai 500 lewat
        // ExceptionHandlingMiddleware) — konsisten dengan bagaimana Employee/
        // Department delete diperlakukan di repo lain, tidak ada pengecekan
        // eksplisit tambahan di service.
        await _criteriaRepository.DeleteAsync(criteria);
        _logger.LogInformation("KpiCriteria {Id} dihapus", id);
    }

    // ================= KpiPeriod =================

    public async Task<KpiPeriodResponseDto> CreatePeriodAsync(KpiPeriodCreateDto dto)
    {
        var existing = await _periodRepository.GetByNameYearAsync(dto.Name, dto.Year);
        if (existing is not null)
            throw new BadRequestException($"KpiPeriod '{dto.Name}' untuk tahun {dto.Year} sudah ada.");

        var period = new KpiPeriod { Name = dto.Name, Year = dto.Year, Status = KpiPeriodStatus.Draft };
        await _periodRepository.AddAsync(period);
        _logger.LogInformation("KpiPeriod '{Name}' ({Year}) dibuat", period.Name, period.Year);

        return ToDto(period);
    }

    public async Task<List<KpiPeriodResponseDto>> GetAllPeriodsAsync()
    {
        var periods = await _periodRepository.GetAllAsync();
        return periods.Select(ToDto).ToList();
    }

    public async Task<KpiPeriodResponseDto> GetPeriodByIdAsync(int id)
    {
        var period = await _periodRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"KpiPeriod dengan id {id} tidak ditemukan.");
        return ToDto(period);
    }

    public async Task<KpiPeriodResponseDto> FinalizeAsync(int periodId)
    {
        var period = await _periodRepository.GetByIdAsync(periodId)
            ?? throw new NotFoundException($"KpiPeriod dengan id {periodId} tidak ditemukan.");

        // "Semua bawahan sudah ditinjau" sengaja tidak dicek eksplisit di sini
        // (mis. Manager wajib approve tiap employee satu-satu) — MVP memakai
        // opsi "manager/admin set status periode manual" sesuai bab 10.2 poin 6
        // technical alignment doc, yang menyebut ini masih poin terbuka.
        if (period.Status != KpiPeriodStatus.InReview)
            throw new BadRequestException("KpiPeriod hanya bisa di-Finalize dari status InReview.");

        period.Status = KpiPeriodStatus.Finalized;
        await _periodRepository.UpdateAsync(period);
        _logger.LogInformation("KpiPeriod {Id} di-Finalize", periodId);

        return ToDto(period);
    }

    // ================= Pengisian nilai (Admin) =================

    public async Task<List<EmployeeKpiScoreResponseDto>> FillScoresAsync(int periodId, int filledByUserId, EmployeeKpiScoreFillDto dto)
    {
        var period = await _periodRepository.GetByIdAsync(periodId)
            ?? throw new NotFoundException($"KpiPeriod dengan id {periodId} tidak ditemukan.");

        if (period.Status == KpiPeriodStatus.Finalized)
            throw new BadRequestException("KpiPeriod ini sudah Finalized, nilai tidak bisa diisi/diubah lagi.");

        var employee = await _employeeRepository.GetByIdAsync(dto.EmployeeId)
            ?? throw new NotFoundException($"Employee dengan id {dto.EmployeeId} tidak ditemukan.");

        foreach (var item in dto.Scores)
        {
            var criteria = await _criteriaRepository.GetByIdAsync(item.CriteriaId)
                ?? throw new NotFoundException($"KpiCriteria dengan id {item.CriteriaId} tidak ditemukan.");

            var existing = await _scoreRepository.GetByPeriodEmployeeCriteriaAsync(periodId, employee.Id, item.CriteriaId);
            if (existing is not null)
            {
                if (existing.Revisions.Count > 0)
                {
                    // Nilai ini sudah pernah di-override Manager sebelumnya.
                    // Admin masih boleh isi ulang (mis. koreksi input awal),
                    // tapi ini di-log supaya jejaknya kelihatan — beda dengan
                    // override Manager, pengisian ulang oleh Admin di sini
                    // TIDAK otomatis membuat baris KpiScoreRevision baru,
                    // karena ini bukan "override" melainkan re-fill data dasar.
                    _logger.LogWarning(
                        "EmployeeKpiScore {Id} (Employee {EmployeeId}, Criteria {CriteriaId}) diisi ulang oleh Admin " +
                        "padahal sudah punya {Count} revisi override dari Manager sebelumnya",
                        existing.Id, employee.Id, item.CriteriaId, existing.Revisions.Count);
                }

                existing.Score = item.Score;
                existing.FilledByUserId = filledByUserId;
                existing.FilledAt = DateTime.UtcNow;
                await _scoreRepository.UpdateAsync(existing);
            }
            else
            {
                var score = new EmployeeKpiScore
                {
                    KpiPeriodId = periodId,
                    EmployeeId = employee.Id,
                    CriteriaId = item.CriteriaId,
                    Score = item.Score,
                    FilledByUserId = filledByUserId,
                    FilledAt = DateTime.UtcNow
                };
                await _scoreRepository.AddAsync(score);
            }
        }

        if (period.Status == KpiPeriodStatus.Draft)
        {
            period.Status = KpiPeriodStatus.InReview;
            await _periodRepository.UpdateAsync(period);
        }

        _logger.LogInformation(
            "EmployeeKpiScore untuk employee {EmployeeId} di KpiPeriod {PeriodId} diisi ({Count} kriteria)",
            employee.Id, periodId, dto.Scores.Count);

        var updated = await _scoreRepository.GetByPeriodAndEmployeeAsync(periodId, employee.Id);
        return updated.Select(ToDto).ToList();
    }

    public async Task<List<EmployeeKpiScoreResponseDto>> GetScoresForPeriodAsync(int periodId)
    {
        _ = await _periodRepository.GetByIdAsync(periodId)
            ?? throw new NotFoundException($"KpiPeriod dengan id {periodId} tidak ditemukan.");

        var scores = await _scoreRepository.GetByPeriodAsync(periodId);
        return scores.Select(ToDto).ToList();
    }

    // ================= Review & override (Manager) =================

    public async Task<List<EmployeeKpiSummaryDto>> GetPendingReviewForManagerAsync(int periodId, int managerEmployeeId)
    {
        _ = await _periodRepository.GetByIdAsync(periodId)
            ?? throw new NotFoundException($"KpiPeriod dengan id {periodId} tidak ditemukan.");

        var scores = await _scoreRepository.GetByPeriodForSubordinatesAsync(periodId, managerEmployeeId);

        return scores
            .GroupBy(s => s.EmployeeId)
            .Select(g => new EmployeeKpiSummaryDto
            {
                EmployeeId = g.Key,
                EmployeeName = g.First().Employee?.FullName ?? string.Empty,
                KpiPeriodId = periodId,
                FinalScore = ComputeFinalScore(g.ToList()),
                Scores = g.OrderBy(s => s.CriteriaId).Select(ToDto).ToList()
            })
            .OrderBy(s => s.EmployeeName)
            .ToList();
    }

    public async Task<EmployeeKpiSummaryDto> GetEmployeeSummaryAsync(int periodId, int employeeId)
    {
        _ = await _periodRepository.GetByIdAsync(periodId)
            ?? throw new NotFoundException($"KpiPeriod dengan id {periodId} tidak ditemukan.");

        var employee = await _employeeRepository.GetByIdAsync(employeeId)
            ?? throw new NotFoundException($"Employee dengan id {employeeId} tidak ditemukan.");

        var scores = await _scoreRepository.GetByPeriodAndEmployeeAsync(periodId, employeeId);

        return new EmployeeKpiSummaryDto
        {
            EmployeeId = employee.Id,
            EmployeeName = employee.FullName,
            KpiPeriodId = periodId,
            FinalScore = ComputeFinalScore(scores),
            Scores = scores.Select(ToDto).ToList()
        };
    }

    public async Task<EmployeeKpiScoreResponseDto> OverrideScoreAsync(int scoreId, int managerEmployeeId, int revisedByUserId, KpiScoreOverrideDto dto)
    {
        var score = await _scoreRepository.GetByIdAsync(scoreId)
            ?? throw new NotFoundException($"EmployeeKpiScore dengan id {scoreId} tidak ditemukan.");

        if (score.Employee?.ManagerId != managerEmployeeId)
            throw new ForbiddenException("Anda bukan manager langsung dari employee pemilik nilai KPI ini.");

        if (score.KpiPeriod?.Status != KpiPeriodStatus.InReview)
            throw new BadRequestException("Override hanya bisa dilakukan selama KpiPeriod berstatus InReview.");

        var revision = new KpiScoreRevision
        {
            EmployeeKpiScoreId = score.Id,
            PreviousScore = score.Score,
            NewScore = dto.NewScore,
            RevisedByUserId = revisedByUserId,
            RevisedAt = DateTime.UtcNow,
            Note = dto.Note
        };

        score.Score = dto.NewScore;
        await _scoreRepository.OverrideAsync(score, revision);

        _logger.LogInformation(
            "EmployeeKpiScore {Id} di-override oleh manager (employee {ManagerEmployeeId}): {Previous} -> {New}",
            scoreId, managerEmployeeId, revision.PreviousScore, revision.NewScore);

        var updated = await _scoreRepository.GetByIdAsync(scoreId);
        return ToDto(updated!);
    }

    public async Task<decimal> CalculateFinalScoreAsync(int employeeId, int periodId)
    {
        var scores = await _scoreRepository.GetByPeriodAndEmployeeAsync(periodId, employeeId);
        return ComputeFinalScore(scores);
    }

    // Σ (Score × Weight) / 100 — Weight disimpan dalam persen (0-100),
    // jadi hasilnya tetap dalam skala yang sama dengan Score (bukan dikali
    // ulang jadi ratusan).
    private static decimal ComputeFinalScore(List<EmployeeKpiScore> scores) =>
        scores.Sum(s => s.Score * (s.Criteria?.Weight ?? 0)) / 100m;

    private static KpiCriteriaResponseDto ToDto(KpiCriteria c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        Weight = c.Weight
    };

    private static KpiPeriodResponseDto ToDto(KpiPeriod p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Year = p.Year,
        Status = p.Status.ToString(),
        CreatedAt = p.CreatedAt
    };

    private static EmployeeKpiScoreResponseDto ToDto(EmployeeKpiScore s) => new()
    {
        Id = s.Id,
        KpiPeriodId = s.KpiPeriodId,
        EmployeeId = s.EmployeeId,
        EmployeeName = s.Employee?.FullName ?? string.Empty,
        CriteriaId = s.CriteriaId,
        CriteriaName = s.Criteria?.Name ?? string.Empty,
        CriteriaWeight = s.Criteria?.Weight ?? 0,
        Score = s.Score,
        FilledByUserId = s.FilledByUserId,
        FilledByUsername = s.FilledByUser?.Username ?? string.Empty,
        FilledAt = s.FilledAt,
        Revisions = s.Revisions
            .OrderBy(r => r.RevisedAt)
            .Select(r => new KpiScoreRevisionResponseDto
            {
                Id = r.Id,
                PreviousScore = r.PreviousScore,
                NewScore = r.NewScore,
                RevisedByUserId = r.RevisedByUserId,
                RevisedByUsername = r.RevisedByUser?.Username ?? string.Empty,
                RevisedAt = r.RevisedAt,
                Note = r.Note
            }).ToList()
    };
}
