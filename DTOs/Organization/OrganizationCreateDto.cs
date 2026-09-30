using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.DTOs.Organization;

public class OrganizationCreateDto
{
    [Required, MaxLength(150)]
    public string OrganizationName { get; set; } = string.Empty;

    // Null berarti organisasi paling atas.
    public int? ParentId { get; set; }
}
