using System.ComponentModel.DataAnnotations;
namespace HRIS.Api.DTOs.Family;
public class FamilyChangeCreateDto { public int? FamilyId { get; set; } [Required, MaxLength(20)] public string Action { get; set; } = "Add"; [Required, MaxLength(150)] public string FamilyName { get; set; } = string.Empty; [Range(1,int.MaxValue)] public int RelationshipId { get; set; } public int? GenderId { get; set; } public DateOnly? BirthDate { get; set; } public bool IsDependent { get; set; } public DateOnly? WeddingDate { get; set; } }
