
using System.ComponentModel.DataAnnotations;
using RaceDay.Api.DTOs;
using Xunit;

namespace RaceDay.Tests;

public class ResultsTests
{
    [Fact]
    public void ResultDto_WithValidData_PassesValidation()
    {
        var dto = new ResultDto
        {
            FinishPosition = 1,
            FinishTime = new TimeSpan(0, 45, 30),
            ResultStatus = "Finished"
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
