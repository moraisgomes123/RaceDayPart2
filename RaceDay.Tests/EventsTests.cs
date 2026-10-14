
using System.ComponentModel.DataAnnotations;
using RaceDay.Api.DTOs;
using Xunit;

namespace RaceDay.Tests;

public class EventsTests
{
    [Fact]
    public void EventDto_WithValidData_PassesValidation()
    {
        var dto = new EventDto
        {
            EventName = "Cape Town Running Event",
            EventType = "Running",
            Description = "A local running event",
            EventDate = DateTime.Today.AddDays(30),
            StartTime = new TimeSpan(8, 0, 0),
            Location = "Cape Town"
        };

        var results = new List<ValidationResult>();
        var context = new ValidationContext(dto);

        Validator.TryValidateObject(
            dto,
            context,
            results,
            validateAllProperties: true);

        Assert.Empty(results);
    }
}
