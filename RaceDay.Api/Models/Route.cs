namespace RaceDay.Api.Models;

public class Route
{
    public int RouteID { get; set; }

    public int EventID { get; set; }

    public string RouteName { get; set; } = string.Empty;

    public decimal DistanceKm { get; set; }

    public string StartPoint { get; set; } = string.Empty;

    public string FinishPoint { get; set; } = string.Empty;

    public string? RouteInformation { get; set; }

    public Event? Event { get; set; }
}