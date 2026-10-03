using HRIS.Api.Common;
using HRIS.Api.Exceptions;
using HRIS.Api.Models;
using HRIS.Api.Models.Enums;
using HRIS.Api.Repositories;
namespace HRIS.Api.Services;
public class ManpowerApprovalHandler : IApprovalHandler
{
    private readonly IManpowerRepository _repo;
    public ManpowerApprovalHandler(IManpowerRepository repo)=>_repo=repo;
    public string RequestType=>ApprovalRequestTypes.Manpower;
    public async Task OnCompletedAsync(ApprovalRequest request, ApprovalRequestStatus finalStatus)
    {
        var x=await _repo.GetByIdAsync(request.RequestRefId) ?? throw new NotFoundException("Permintaan tenaga kerja tidak ditemukan.");
        x.Status=finalStatus.ToString();
        x.ApprovalPhase = finalStatus == ApprovalRequestStatus.Approved ? "Done Approval" : finalStatus.ToString();
        await _repo.UpdateAsync(x);
        if (finalStatus == ApprovalRequestStatus.Approved && !x.Vacancies.Any())
        {
            for (var i = 1; i <= x.Headcount; i++)
                x.Vacancies.Add(new ManpowerVacancy { ManpowerRequestId = x.Id, VacancyCode = $"MRF-{x.Id:D6}-{i:D2}", PositionNo = i, Status = "Open" });
            await _repo.UpdateAsync(x);
        }
    }
}
