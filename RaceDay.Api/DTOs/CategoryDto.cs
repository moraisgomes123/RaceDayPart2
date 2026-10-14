using System.ComponentModel.DataAnnotations;

namespace RaceDay.Api.DTOs;

public class CategoryDto
{
    [Required, MaxLength(100)]
    public string CategoryName { get; set; } = string.Empty;

    [Range(0.01, 9999)]
    public decimal? DistanceKm { get; set; }

    [Range(1, int.MaxValue)]
    public int? MaximumParticipants { get; set; }

    [Range(0, 99999999)]
    public decimal EntryFee { get; set; }
}