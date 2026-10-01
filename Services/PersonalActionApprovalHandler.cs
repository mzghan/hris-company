using HRIS.Api.Common;
using HRIS.Api.Exceptions;
using HRIS.Api.Models;
using HRIS.Api.Models.Enums;
using HRIS.Api.Repositories;
namespace HRIS.Api.Services;
public class PersonalActionApprovalHandler:IApprovalHandler
{
 private readonly IPersonalActionRepository _repo;
 public PersonalActionApprovalHandler(IPersonalActionRepository repo)=>_repo=repo;
 public string RequestType=>ApprovalRequestTypes.PersonalAction;
 public async Task OnCompletedAsync(ApprovalRequest request,ApprovalRequestStatus finalStatus)
 {
   var x=await _repo.GetByIdAsync(request.RequestRefId)??throw new NotFoundException("Personal Action tidak ditemukan.");
   if(finalStatus==ApprovalRequestStatus.Approved) await _repo.ApplyApprovedAsync(x);
   else x.Status=finalStatus.ToString();
   await _repo.UpdateAsync(x);
 }
}