using System.ComponentModel.DataAnnotations;

namespace HRIS.Api.Models;

// REF_Grade. Seed angka 1 sampai 8 (makin besar makin tinggi). GradeLevel dipakai untuk perbandingan di Service.
public class Grade
{
    public int Id { get; set; }

    [MaxLength(200)]
    public string? GradeDescription { get; set; }

    public int GradeLevel { get; set; }
}
