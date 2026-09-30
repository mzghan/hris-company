using HRIS.Api.Common;
using HRIS.Api.Exceptions;
using HRIS.Api.Models;
using HRIS.Api.Models.Enums;
using HRIS.Api.Repositories;
namespace HRIS.Api.Services;
public class HealthClaimApprovalHandler : IApprovalHandler
{
    private readonly IFlexibleBenefitRepository _repo;
    public HealthClaimApprovalHandler(IFlexibleBenefitRepository repo)=>_repo=repo;
    public string RequestType=>ApprovalRequestTypes.HealthClaim;
    public async Task OnCompletedAsync(ApprovalRequest request, ApprovalRequestStatus finalStatus)
    {
        var x=await _repo.GetClaimAsync(request.RequestRefId) ?? throw new NotFoundException("Klaim kesehatan tidak ditemukan.");
        x.Status=finalStatus.ToString();
        await _repo.SaveAsync();
    }
}
