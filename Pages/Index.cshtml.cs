using HRIS.Api.Controllers;
using HRIS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HRIS.Api.Pages;

[Authorize(AuthenticationSchemes = "Cookies")]
public class IndexModel : PageModel
{
    private readonly IEmployeeService _employeeService;
    private readonly ILeaveRequestService _leaveService;
    private readonly IAttendanceService _attendanceService;
    private readonly IPayrollService _payrollService;

    public IndexModel(
        IEmployeeService employeeService,
        ILeaveRequestService leaveService,
        IAttendanceService attendanceService,
        IPayrollService payrollService)
    {
        _employeeService = employeeService;
        _leaveService = leaveService;
        _attendanceService = attendanceService;
        _payrollService = payrollService;
    }

    public string Role { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public bool HasEmployeeProfile { get; set; }

    public int? TotalEmployees { get; set; }
    public int? PendingLeaveApprovals { get; set; }
    public int? PendingPayrollApprovals { get; set; }
    public int? MyPendingLeaveCount { get; set; }
    public bool CheckedInToday { get; set; }
    public bool CheckedOutToday { get; set; }

    public async Task OnGetAsync()
    {
        Role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? string.Empty;
        Username = User.Identity?.Name ?? string.Empty;
        HasEmployeeProfile = User.HasClaim(c => c.Type == "employeeId");

        if (Role == "Admin")
        {
            TotalEmployees = (await _employeeService.GetAllAsync()).Count;
        }

        if (Role is "Manager" or "Admin" && HasEmployeeProfile)
        {
            var employeeId = User.GetEmployeeId();
            PendingLeaveApprovals = (await _leaveService.GetPendingForApproverAsync(employeeId)).Count;
            PendingPayrollApprovals = (await _payrollService.GetPendingForApproverAsync(employeeId)).Count;
        }

        if (HasEmployeeProfile)
        {
            var employeeId = User.GetEmployeeId();
            var myLeaves = await _leaveService.GetMyRequestsAsync(employeeId);
            MyPendingLeaveCount = myLeaves.Count(l => l.Status == "Pending");

            var today = DateOnly.FromDateTime(DateTime.Today);
            var history = await _attendanceService.GetHistoryAsync(employeeId);
            var todayRecord = history.FirstOrDefault(a => a.Date == today);
            CheckedInToday = todayRecord?.CheckIn is not null;
            CheckedOutToday = todayRecord?.CheckOut is not null;
        }
    }
}
