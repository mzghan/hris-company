using HRIS.Api.Models;

namespace HRIS.Api.Repositories;

public interface IEmployeeKpiScoreRepository
{
    Task<EmployeeKpiScore?> GetByIdAsync(int id);
    Task<List<EmployeeKpiScore>> GetByPeriodAsync(int kpiPeriodId);
    Task<List<EmployeeKpiScore>> GetByPeriodAndEmployeeAsync(int kpiPeriodId, int employeeId);
    Task<EmployeeKpiScore?> GetByPeriodEmployeeCriteriaAsync(int kpiPeriodId, int employeeId, int criteriaId);

    // Semua EmployeeKpiScore pada satu periode, milik bawahan langsung
    // dari managerEmployeeId (dicocokkan lewat Employee.ManagerId — pola
    // yang sama seperti dipakai LeaveApproval/PayrollApproval untuk
    // menentukan siapa yang berhak bertindak).
    Task<List<EmployeeKpiScore>> GetByPeriodForSubordinatesAsync(int kpiPeriodId, int managerEmployeeId);

    Task<EmployeeKpiScore> AddAsync(EmployeeKpiScore score);
    Task UpdateAsync(EmployeeKpiScore score);

    // Override: update EmployeeKpiScore.Score DAN insert KpiScoreRevision
    // dalam satu SaveChanges, supaya atomik (tidak ada state "nilai sudah
    // berubah tapi revision gagal tersimpan").
    Task OverrideAsync(EmployeeKpiScore score, KpiScoreRevision revision);
}
