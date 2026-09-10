using HRIS.Api.DTOs.Payroll;

namespace HRIS.Api.Services;

public interface IPayrollService
{
    Task<PayrollPeriodResponseDto> CreatePeriodAsync(PayrollPeriodCreateDto dto);
    Task<List<PayrollPeriodResponseDto>> GetAllAsync();
    Task<PayrollPeriodResponseDto> GetByIdAsync(int id);
    Task<List<PayrollPeriodResponseDto>> GetPendingForApproverAsync(int approverEmployeeId);
    Task<PayrollPeriodResponseDto> ApproveAsync(int periodId, int approverEmployeeId, PayrollApprovalActionDto dto);
    Task<PayrollPeriodResponseDto> RejectAsync(int periodId, int approverEmployeeId, PayrollApprovalActionDto dto);
    Task<PayrollPeriodResponseDto> MarkPaidAsync(int periodId);
}
