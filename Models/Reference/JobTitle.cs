using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.Models;

// REF_Job_Title
public class JobTitle
{
    public int Id { get; set; }

    [Required, MaxLength(150)]
    public string JobTitleName { get; set; } = string.Empty;
}
