using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.Models;

// REF_Job_Level. LevelOrder dipakai untuk mengurutkan jenjang (makin besar makin tinggi).
public class JobLevel
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string JobLevelName { get; set; } = string.Empty;

    public int LevelOrder { get; set; }
}
