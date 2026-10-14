using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.DTOs;
using RaceDay.Api.Models;
using System.Security.Claims;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api/events")]
public class EventsController : ControllerBase
{
    private readonly RaceDayContext _db;

    public EventsController(RaceDayContext db)
    {
        _db = db;
    }

    private int? CurrentUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(value, out int id) ? id : null;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var events = await _db.Events.AsNoTracking()
            .Select(x => new
            {
                x.EventID,
                x.EventName,
                x.EventType,
                x.Description,
                x.EventDate,
                x.StartTime,
                x.Location,
                x.OrganiserID
            }).ToListAsync();

        return Ok(events);
    }

    [HttpGet("{eventId:int}")]
    public async Task<IActionResult> GetById(int eventId)
    {
        var item = await _db.Events.AsNoTracking()
            .Where(x => x.EventID == eventId)
            .Select(x => new
            {
                x.EventID,
                x.EventName,
                x.EventType,
                x.Description,
                x.EventDate,
                x.StartTime,
                x.Location,
                x.OrganiserID
            }).SingleOrDefaultAsync();

        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Create(EventDto dto)
    {
        var organiserId = CurrentUserId();
        if (organiserId == null) return Unauthorized();

        if (!new[] { "Running", "Walking", "Cycling" }
            .Contains(dto.EventType, StringComparer.OrdinalIgnoreCase))
        {
            return BadRequest(new { message = "Invalid event type." });
        }

        var item = new Event
        {
            EventName = dto.EventName.Trim(),
            EventType = dto.EventType.Trim(),
            Description = dto.Description,
            EventDate = dto.EventDate.Date,
            StartTime = dto.StartTime,
            Location = dto.Location.Trim(),
            OrganiserID = organiserId.Value,
            CreatedAt = DateTime.UtcNow
        };

        _db.Events.Add(item);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById),
            new { eventId = item.EventID },
            new { item.EventID, item.EventName, item.EventDate });
    }

    [HttpPut("{eventId:int}")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Update(int eventId, EventDto dto)
    {
        var organiserId = CurrentUserId();
        if (organiserId == null) return Unauthorized();

        var item = await _db.Events.FindAsync(eventId);
        if (item == null) return NotFound();
        if (item.OrganiserID != organiserId.Value) return Forbid();

        if (!new[] { "Running", "Walking", "Cycling" }
            .Contains(dto.EventType, StringComparer.OrdinalIgnoreCase))
        {
            return BadRequest(new { message = "Invalid event type." });
        }

        item.EventName = dto.EventName.Trim();
        item.EventType = dto.EventType.Trim();
        item.Description = dto.Description;
        item.EventDate = dto.EventDate.Date;
        item.StartTime = dto.StartTime;
        item.Location = dto.Location.Trim();

        await _db.SaveChangesAsync();
        return Ok(new { message = "Event updated successfully." });
    }

    [HttpDelete("{eventId:int}")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Delete(int eventId)
    {
        var organiserId = CurrentUserId();
        if (organiserId == null) return Unauthorized();

        var item = await _db.Events.FindAsync(eventId);
        if (item == null) return NotFound();
        if (item.OrganiserID != organiserId.Value) return Forbid();

        bool hasEnrolments = await _db.Enrolments
            .AnyAsync(x => x.EventID == eventId);

        bool hasRoute = await _db.Routes
            .AnyAsync(x => x.EventID == eventId);

        bool hasCategories = await _db.Categories
            .AnyAsync(x => x.EventID == eventId);

        if (hasEnrolments || hasRoute || hasCategories)
        {
            return Conflict(new
            {
                message = "Remove related records before deleting this event."
            });
        }

        _db.Events.Remove(item);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}