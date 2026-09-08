namespace HRIS.Api.DTOs.Attendance;

// EmployeeId diambil dari JWT claim yang sedang login, bukan dari body,
// supaya seorang employee tidak bisa absen atas nama orang lain.
public class AttendanceCheckInDto
{
}
