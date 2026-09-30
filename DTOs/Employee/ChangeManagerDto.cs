namespace HRIS.Api.DTOs.Employee;

public class ChangeManagerDto
{
    // Null = hapus atasan langsung.
    public int? ManagerId { get; set; }
    public DateOnly EffectiveDate { get; set; }
}
