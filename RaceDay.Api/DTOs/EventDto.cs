using System.ComponentModel.DataAnnotations;

namespace RaceDay.Api.DTOs;

public class EventDto
{
    [Required, MaxLength(150)]
    public string EventName { get; set; } = string.Empty;

    [Required]
    public string EventType { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    public DateTime EventDate { get; set; }

    public TimeSpan StartTime { get; set; }

    [Required, MaxLength(200)]
    public string Location { get; set; } = string.Empty;
}