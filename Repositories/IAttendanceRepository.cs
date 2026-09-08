using HRIS.Api.Models;

namespace HRIS.Api.Repositories;

public interface IAttendanceRepository
{
    Task<Attendance?> GetByEmployeeAndDateAsync(int employeeId, DateOnly date);
    Task<List<Attendance>> GetByEmployeeAsync(int employeeId);
    Task<Attendance> AddAsync(Attendance attendance);
    Task UpdateAsync(Attendance attendance);
}
