using HRIS.Api.DTOs.FlexibleBenefit;
using HRIS.Api.Common;
namespace HRIS.Api.Services;
public interface IFlexibleBenefitService
{
    Task<List<LeaveBalanceResponseDto>> GetBalancesAsync(UserContext actor, int year);
    Task<List<FlexPeriodResponseDto>> GetPeriodsAsync(bool openOnly=false);
    Task<List<LeaveEncashmentResponseDto>> GetEncashmentsAsync(UserContext actor);
    Task<LeaveEncashmentResponseDto> CreateEncashmentAsync(LeaveEncashmentCreateDto dto, UserContext actor);
    Task<List<HealthClaimResponseDto>> GetHealthClaimsAsync(UserContext actor);
    Task<HealthClaimResponseDto> CreateHealthClaimAsync(HealthClaimCreateDto dto, UserContext actor);
    Task UpdateHealthClaimStatusAsync(int id, string status, UserContext actor);
}
