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
    private readonly IApprovalService _approvalService;

    public IndexModel(
        IEmployeeService employeeService,
        ILeaveRequestService leaveService,
        IAttendanceService attendanceService,
        IApprovalService approvalService)
    {
        _employeeService = employeeService;
        _leaveService = leaveService;
        _attendanceService = attendanceService;
        _approvalService = approvalService;
    }

    public string Role { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public bool HasEmployeeProfile { get; set; }
    public bool IsHrOrSupport { get; set; }
    public bool CanApprove { get; set; }

    public int? TotalEmployees { get; set; }
    public int PendingApprovals { get; set; }
    public int? MyPendingLeaveCount { get; set; }
    public bool CheckedInToday { get; set; }
    public bool CheckedOutToday { get; set; }

    public async Task OnGetAsync()
    {
        Role = string.Join(", ", User.FindAll(System.Security.Claims.ClaimTypes.Role).Select(c => c.Value));
        IsHrOrSupport = User.IsHrOrSupport();
        CanApprove = IsHrOrSupport || User.IsManager();
        Username = User.Identity?.Name ?? string.Empty;
        HasEmployeeProfile = User.HasClaim(c => c.Type == "employeeId");

        if (IsHrOrSupport)
        {
            TotalEmployees = (await _employeeService.GetAllAsync()).Count;
        }

        // Inbox approval berlaku untuk semua jenis pengajuan (approver bisa juga bertipe Role, mis. HR).
        PendingApprovals = (await _approvalService.GetInboxAsync(User.ToUserContext())).Count;

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
