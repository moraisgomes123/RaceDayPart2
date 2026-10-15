using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.DTOs;
using System.Security.Claims;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly RaceDayContext _db;

    public UsersController(RaceDayContext db)
    {
        _db = db;
    }

    private int? CurrentUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(value, out int id) ? id : null;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var id = CurrentUserId();
        if (id == null) return Unauthorized();

        var user = await _db.Users
            .Where(x => x.UserID == id.Value)
            .Select(x => new
            {
                x.UserID,
                x.FirstName,
                x.LastName,
                x.Email,
                x.Phone,
                x.Role
            })
            .SingleOrDefaultAsync();

        return user == null ? NotFound() : Ok(user);
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateMe(UpdateUserDto dto)
    {
        var id = CurrentUserId();
        if (id == null) return Unauthorized();

        var user = await _db.Users.FindAsync(id.Value);
        if (user == null) return NotFound();

        var email = dto.Email.Trim().ToLowerInvariant();

        if (await _db.Users.AnyAsync(
            x => x.Email == email && x.UserID != id.Value))
        {
            return Conflict(new { message = "Email already registered." });
        }

        user.FirstName = dto.FirstName.Trim();
        user.LastName = dto.LastName.Trim();
        user.Email = email;
        user.Phone = dto.Phone;

        await _db.SaveChangesAsync();

        return Ok(new
        {
            user.UserID,
            user.FirstName,
            user.LastName,
            user.Email,
            user.Phone,
            user.Role
        });
    }
}
