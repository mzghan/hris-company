using HRIS.Api.DTOs.Kpi;

namespace HRIS.Api.Services;

public interface IKpiService
{
    // --- KpiCriteria (master data, dikelola Admin) ---
    Task<KpiCriteriaResponseDto> CreateCriteriaAsync(KpiCriteriaCreateDto dto);
    Task<List<KpiCriteriaResponseDto>> GetAllCriteriaAsync();
    Task<KpiCriteriaResponseDto> UpdateCriteriaAsync(int id, KpiCriteriaUpdateDto dto);
    Task DeleteCriteriaAsync(int id);

    // --- KpiPeriod ---
    Task<KpiPeriodResponseDto> CreatePeriodAsync(KpiPeriodCreateDto dto);
    Task<List<KpiPeriodResponseDto>> GetAllPeriodsAsync();
    Task<KpiPeriodResponseDto> GetPeriodByIdAsync(int id);
    Task<KpiPeriodResponseDto> FinalizeAsync(int periodId);

    // --- Pengisian nilai oleh Admin ---
    Task<List<EmployeeKpiScoreResponseDto>> FillScoresAsync(int periodId, int filledByUserId, EmployeeKpiScoreFillDto dto);
    Task<List<EmployeeKpiScoreResponseDto>> GetScoresForPeriodAsync(int periodId);

    // --- Review & override oleh Manager ---
    Task<List<EmployeeKpiSummaryDto>> GetPendingReviewForManagerAsync(int periodId, int managerEmployeeId);
    Task<EmployeeKpiSummaryDto> GetEmployeeSummaryAsync(int periodId, int employeeId);
    Task<EmployeeKpiScoreResponseDto> OverrideScoreAsync(int scoreId, int managerEmployeeId, int revisedByUserId, KpiScoreOverrideDto dto);

    // Σ (Score × Weight) / 100 dari semua KpiCriteria — dihitung
    // on-the-fly, bukan disimpan di tabel tambahan (lihat bab 10.2 poin 5).
    Task<decimal> CalculateFinalScoreAsync(int employeeId, int periodId);
}
