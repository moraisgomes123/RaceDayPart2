
using System.ComponentModel.DataAnnotations;
using RaceDay.Api.DTOs;
using Xunit;

namespace RaceDay.Tests;

public class EnrolmentsTests
{
    [Fact]
    public void EnrolmentDto_WithValidIdentifiers_PassesValidation()
    {
        var dto = new EnrolmentDto
        {
            CategoryID = 1
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
