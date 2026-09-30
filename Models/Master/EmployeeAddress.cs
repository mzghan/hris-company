using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.Models;

// MST_Employee_Address
public class EmployeeAddress : IAuditable
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    public int AddressTypeId { get; set; }
    public AddressType? AddressType { get; set; }

    [MaxLength(500)]
    public string? AddressDetail { get; set; }

    public int? CityId { get; set; }
    public City? City { get; set; }

    public bool IsPrimary { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
}
