using System.ComponentModel.DataAnnotations;
namespace HRIS.Api.DTOs.Letter;
public class LetterRequestCreateDto { [Range(1,int.MaxValue)] public int LetterTypeId { get; set; } [Required, MaxLength(500)] public string Purpose { get; set; } = string.Empty; [MaxLength(300)] public string? Destination { get; set; } }
