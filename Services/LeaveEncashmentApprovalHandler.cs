using HRIS.Api.Common;
using HRIS.Api.Exceptions;
using HRIS.Api.Models;
using HRIS.Api.Models.Enums;
using HRIS.Api.Repositories;
namespace HRIS.Api.Services;
public class LeaveEncashmentApprovalHandler : IApprovalHandler
{
    private readonly IFlexibleBenefitRepository _repo;
    public LeaveEncashmentApprovalHandler(IFlexibleBenefitRepository repo)=>_repo=repo;
    public string RequestType=>ApprovalRequestTypes.LeaveEncashment;
    public async Task OnCompletedAsync(ApprovalRequest request, ApprovalRequestStatus finalStatus)
    {
        var x=await _repo.GetEncashmentAsync(request.RequestRefId) ?? throw new NotFoundException("Pengajuan penjualan cuti tidak ditemukan.");
        x.Status=finalStatus.ToString();
        if(finalStatus==ApprovalRequestStatus.Approved)
        {
            var balance=await _repo.GetBalanceAsync(x.EmployeeId,x.LeaveTypeId,x.Period!.StartDate.Year)
                ?? throw new BadRequestException("Saldo cuti tidak ditemukan.");
            var available=balance.Entitlement+balance.CarriedOver-balance.Used-balance.Sold;
            if(available<x.Days) throw new BadRequestException("Saldo cuti tidak cukup saat approval.");
            balance.Sold+=x.Days;
        }
        await _repo.SaveAsync();
    }
}
