using System.ComponentModel.DataAnnotations;
using RaceDay.Api.DTOs;
using Xunit;

namespace RaceDay.Tests;

public class AuthTests
{
    [Fact]
    public void RegisterDto_WithValidData_PassesValidation()
    {
        var dto = new RegisterDto
        {
            FirstName = "Morais",
            LastName = "Adao",
            Email = "morais@example.com",
            Password = "StrongPassword123!",
            Phone = "986583523"
        };

        var results = Validate(dto);

        Assert.Empty(results);
    }

    [Fact]
    public void RegisterDto_WithInvalidEmail_FailsValidation()
    {
        var dto = new RegisterDto
        {
            FirstName = "Morais",
            LastName = "Adao",
            Email = "not-an-email",
            Password = "StrongPassword123!",
            Phone = "986583523"
        };

        var results = Validate(dto);

        Assert.NotEmpty(results);
    }

    [Fact]
    public void LoginDto_WithValidData_PassesValidation()
    {
        var dto = new LoginDto
        {
            Email = "morais@example.com",
            Password = "StrongPassword123!"
        };

        var results = Validate(dto);

        Assert.Empty(results);
    }

    private static List<ValidationResult> Validate(object model)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(model);

        Validator.TryValidateObject(
            model,
            context,
            results,
            validateAllProperties: true);

        return results;
    }
}
