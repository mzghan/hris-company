using HRIS.Api.Models;

namespace HRIS.Api.Repositories;

public interface IPayrollPeriodRepository
{
    Task<List<PayrollPeriod>> GetAllAsync();
    Task<PayrollPeriod?> GetByIdAsync(int id);
    Task<PayrollPeriod?> GetByMonthYearAsync(int month, int year);

    // Periode yang sedang menunggu approval dari approverEmployeeId tertentu
    // (dicocokkan lewat PayrollApproval.ApproverId + CurrentLevel) — pola
    // sama seperti ILeaveRequestRepository.GetPendingForApproverAsync.
    Task<List<PayrollPeriod>> GetPendingForApproverAsync(int approverEmployeeId);

    Task<PayrollPeriod> AddAsync(PayrollPeriod period);
    Task UpdateAsync(PayrollPeriod period);
}
