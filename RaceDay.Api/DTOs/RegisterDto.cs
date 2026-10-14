using System.ComponentModel.DataAnnotations;

namespace RaceDay.Api.DTOs;

public class RegisterDto
{
    [Required, MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(8)]
    public string Password { get; set; } = string.Empty;

    [Phone, MaxLength(30)]
    public string? Phone { get; set; }
}
