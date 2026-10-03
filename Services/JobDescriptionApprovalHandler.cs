using HRIS.Api.Common;
using HRIS.Api.Exceptions;
using HRIS.Api.Models;
using HRIS.Api.Models.Enums;
using HRIS.Api.Repositories;
namespace HRIS.Api.Services;
public class JobDescriptionApprovalHandler : IApprovalHandler
{
    private readonly IJobDescriptionRepository _repo;
    public JobDescriptionApprovalHandler(IJobDescriptionRepository repo)=>_repo=repo;
    public string RequestType=>ApprovalRequestTypes.JobDescription;
    public async Task OnCompletedAsync(ApprovalRequest request, ApprovalRequestStatus finalStatus)
    {
        var x=await _repo.GetByIdAsync(request.RequestRefId)??throw new NotFoundException("JD tidak ditemukan.");
        x.ApprovalFlag=finalStatus==ApprovalRequestStatus.Approved?5:-1;
        x.Status=finalStatus==ApprovalRequestStatus.Approved?"Awaiting Job Holder Signature":"Rejected";
        await _repo.UpdateAsync(x);
    }
}
