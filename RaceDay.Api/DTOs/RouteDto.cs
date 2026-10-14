using System.ComponentModel.DataAnnotations;

namespace RaceDay.Api.DTOs;

public class RouteDto
{
    [Required, MaxLength(150)]
    public string RouteName { get; set; } = string.Empty;

    [Range(0.01, 9999)]
    public decimal DistanceKm { get; set; }

    [Required, MaxLength(200)]
    public string StartPoint { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string FinishPoint { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? RouteInformation { get; set; }
}