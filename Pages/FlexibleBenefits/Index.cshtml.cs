using HRIS.Api.Controllers;
using System.ComponentModel.DataAnnotations;
using HRIS.Api.Common;
using HRIS.Api.DTOs.FlexibleBenefit;
using HRIS.Api.Exceptions;
using HRIS.Api.Models;
using HRIS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HRIS.Api.Pages.FlexibleBenefits;

[Authorize(AuthenticationSchemes = "Cookies")]
public class IndexModel : PageModel
{
    private readonly IFlexibleBenefitService _service;
    private readonly ILeaveRequestService _leaveService;
    public IndexModel(IFlexibleBenefitService service, ILeaveRequestService leaveService){_service=service;_leaveService=leaveService;}

    public List<LeaveBalanceResponseDto> Balances { get; set; } = new();
    public List<FlexPeriodResponseDto> Periods { get; set; } = new();
    public List<LeaveEncashmentResponseDto> Encashments { get; set; } = new();
    public List<HealthClaimResponseDto> Claims { get; set; } = new();
    public List<LeaveType> LeaveTypes { get; set; } = new();
    public bool CanSubmit => int.TryParse(User.FindFirst("employeeId")?.Value, out _);

    [BindProperty] public EncashmentInput Encashment { get; set; } = new();
    [BindProperty] public ClaimInput Claim { get; set; } = new();

    public class EncashmentInput
    {
        [Required] public int PeriodId { get; set; }
        [Required] public int LeaveTypeId { get; set; }
        [Range(1,365)] public int Days { get; set; }
    }
    public class ClaimInput
    {
        [Required] public int PeriodId { get; set; }
        [Range(1,long.MaxValue)] public long Amount { get; set; }
        [MaxLength(500)] public string? Description { get; set; }
    }

    public async Task OnGetAsync()=>await LoadAsync();

    public async Task<IActionResult> OnPostEncashAsync()
    {
        if(!ModelState.IsValid){await LoadAsync();return Page();}
        try
        {
            await _service.CreateEncashmentAsync(new LeaveEncashmentCreateDto
            { PeriodId=Encashment.PeriodId, LeaveTypeId=Encashment.LeaveTypeId, Days=Encashment.Days }, User.ToUserContext());
            TempData["Success"]="Pengajuan penjualan cuti berhasil dikirim.";
            return RedirectToPage();
        }
        catch(Exception ex) when(ex is BadRequestException or ForbiddenException){ModelState.AddModelError("",ex.Message);await LoadAsync();return Page();}
    }

    public async Task<IActionResult> OnPostClaimAsync()
    {
        if(!ModelState.IsValid){await LoadAsync();return Page();}
        try
        {
            await _service.CreateHealthClaimAsync(new HealthClaimCreateDto
            { PeriodId=Claim.PeriodId, Amount=Claim.Amount, Description=Claim.Description }, User.ToUserContext());
            TempData["Success"]="Klaim kesehatan berhasil dikirim.";
            return RedirectToPage();
        }
        catch(Exception ex) when(ex is BadRequestException or ForbiddenException){ModelState.AddModelError("",ex.Message);await LoadAsync();return Page();}
    }


    public async Task<IActionResult> OnPostHealthStatusAsync(int id,string status)
    {
        try { await _service.UpdateHealthClaimStatusAsync(id,status,User.ToUserContext()); TempData["Success"]="Status klaim diperbarui."; }
        catch(Exception ex) when(ex is BadRequestException or ForbiddenException or NotFoundException){TempData["Error"]=ex.Message;}
        return RedirectToPage();
    }

    private async Task LoadAsync()
    {
        Periods=await _service.GetPeriodsAsync();
        Encashments=await _service.GetEncashmentsAsync(User.ToUserContext());
        Claims=await _service.GetHealthClaimsAsync(User.ToUserContext());
        if(CanSubmit)
        {
            Balances=await _service.GetBalancesAsync(User.ToUserContext(),DateTime.Today.Year);
            // Leave type options are exposed through the standard reference service in the view model below.
            LeaveTypes=await _leaveService.GetLeaveTypesAsync();
        }
    }
}
