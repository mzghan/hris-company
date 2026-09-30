using System.ComponentModel.DataAnnotations;
using HRIS.Api.Controllers;
using HRIS.Api.Common;
using HRIS.Api.Exceptions;
using HRIS.Api.Models;
using HRIS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HRIS.Api.Pages.Leave;

[Authorize(AuthenticationSchemes="Cookies", Roles="HR,Support")]
public class ManageModel : PageModel
{
    private readonly ILeaveAdminService _service;
    public ManageModel(ILeaveAdminService service)=>_service=service;
    public List<LeaveBalance> Balances { get; set; } = new();
    public List<PublicHoliday> Holidays { get; set; } = new();
    public int Year { get; set; }

    [BindProperty] public BalanceInput Balance { get; set; } = new();
    [BindProperty] public HolidayInput Holiday { get; set; } = new();

    public class BalanceInput
    {
        [Required] public int Id { get; set; }
        [Range(0,3650)] public int Entitlement { get; set; }
        [Range(0,3650)] public int Used { get; set; }
        [Range(0,3650)] public int Sold { get; set; }
        [Range(0,3650)] public int CarriedOver { get; set; }
    }
    public class HolidayInput
    {
        [Required] public DateTime Date { get; set; } = DateTime.Today;
        [Required,MaxLength(150)] public string Name { get; set; } = string.Empty;
    }

    public async Task OnGetAsync(int? year=null){Year=year??DateTime.Today.Year;await LoadAsync();}
    public async Task<IActionResult> OnPostBalanceAsync(int year)
    {
        try{await _service.UpdateBalanceAsync(Balance.Id,Balance.Entitlement,Balance.Used,Balance.Sold,Balance.CarriedOver,User.ToUserContext());TempData["Success"]="Saldo cuti diperbarui.";}
        catch(Exception ex) when(ex is BadRequestException or ForbiddenException or NotFoundException){TempData["Error"]=ex.Message;}
        return RedirectToPage(new{year});
    }
    public async Task<IActionResult> OnPostHolidayAsync()
    {
        try{await _service.AddHolidayAsync(DateOnly.FromDateTime(Holiday.Date),Holiday.Name,User.ToUserContext());TempData["Success"]="Hari libur ditambahkan.";}
        catch(Exception ex) when(ex is BadRequestException or ForbiddenException){TempData["Error"]=ex.Message;}
        return RedirectToPage(new{year=Holiday.Date.Year});
    }
    public async Task<IActionResult> OnPostDeleteHolidayAsync(int id,int year)
    {
        try{await _service.DeleteHolidayAsync(id,User.ToUserContext());TempData["Success"]="Hari libur dihapus.";}
        catch(Exception ex) when(ex is BadRequestException or ForbiddenException or NotFoundException){TempData["Error"]=ex.Message;}
        return RedirectToPage(new{year});
    }
    private async Task LoadAsync(){Balances=await _service.GetBalancesAsync(Year,User.ToUserContext());Holidays=await _service.GetHolidaysAsync(Year,User.ToUserContext());}
}
