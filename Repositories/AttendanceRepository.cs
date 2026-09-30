using HRIS.Api.Data;
using HRIS.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HRIS.Api.Repositories;

public class AttendanceRepository : IAttendanceRepository
{
    private readonly AppDbContext _context;

    public AttendanceRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Attendance?> GetByEmployeeAndDateAsync(int employeeId, DateOnly date) =>
        await _context.Attendances
            .Include(a => a.Employee).Include(a => a.WorkType)
            .FirstOrDefaultAsync(a => a.EmployeeId == employeeId && a.Date == date);

    public async Task<List<Attendance>> GetByEmployeeAsync(int employeeId) =>
        await _context.Attendances
            .Include(a => a.Employee).Include(a => a.WorkType)
            .Where(a => a.EmployeeId == employeeId)
            .OrderByDescending(a => a.Date)
            .ToListAsync();

    public async Task<Attendance> AddAsync(Attendance attendance)
    {
        _context.Attendances.Add(attendance);
        await _context.SaveChangesAsync();
        return attendance;
    }

    public async Task UpdateAsync(Attendance attendance)
    {
        _context.Attendances.Update(attendance);
        await _context.SaveChangesAsync();
    }
}
