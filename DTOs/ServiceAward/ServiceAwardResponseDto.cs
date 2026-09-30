namespace HRIS.Api.DTOs.ServiceAward;
public class ServiceAwardResponseDto { public int Id { get; set; } public int EmployeeId { get; set; } public string EmployeeName { get; set; } = string.Empty; public int ServiceYears { get; set; } public DateOnly DueDate { get; set; } public string Status { get; set; } = string.Empty; public DateTime? GivenAt { get; set; } }
