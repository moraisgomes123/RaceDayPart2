using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.DTOs;
using RaceDay.Api.Models;
using RaceDay.Api.Services;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly RaceDayContext _db;
    private readonly TokenService _tokens;
    private readonly PasswordHasher<User> _hasher = new();

    public AuthController(RaceDayContext db, TokenService tokens)
    {
        _db = db;
        _tokens = tokens;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();

        if (await _db.Users.AnyAsync(x => x.Email == email))
        {
            return Conflict(new
            {
                message = "Email already registered."
            });
        }

        var user = new User
        {
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            Email = email,
            Phone = dto.Phone,
            Role = "Participant",
            CreatedAt = DateTime.UtcNow
        };

        user.PasswordHash = _hasher.HashPassword(
            user,
            dto.Password
        );

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return StatusCode(201, new
        {
            user.UserID,
            user.FirstName,
            user.LastName,
            user.Email,
            user.Role
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();

        var user = await _db.Users.SingleOrDefaultAsync(
            x => x.Email == email
        );

        if (user == null)
        {
            return Unauthorized(new
            {
                message = "Invalid email or password."
            });
        }

        PasswordVerificationResult verification;

        try
        {
            verification = _hasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                dto.Password
            );
        }
        catch (FormatException)
        {
            // The stored password hash is malformed.
            return Unauthorized(new
            {
                message = "Invalid email or password."
            });
        }

        if (verification == PasswordVerificationResult.Failed)
        {
            return Unauthorized(new
            {
                message = "Invalid email or password."
            });
        }

        // Update older hashes if the hasher recommends it.
        if (verification == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash = _hasher.HashPassword(
                user,
                dto.Password
            );

            await _db.SaveChangesAsync();
        }

        return Ok(new
        {
            token = _tokens.CreateToken(user),
            user.UserID,
            user.FirstName,
            user.LastName,
            user.Email,
            user.Role
        });
    }
}
