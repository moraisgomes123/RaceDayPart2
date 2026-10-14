using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.DTOs;
using RaceDay.Api.Models;
using System.Security.Claims;

namespace RaceDay.Api.Controllers;

[ApiController]
public class EnrolmentsController : ControllerBase
{
    private readonly RaceDayContext _db;

    public EnrolmentsController(RaceDayContext db) => _db = db;

    private int? UserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(value, out int id) ? id : null;
    }

    [HttpPost("api/events/{eventId:int}/enrolments")]
    [Authorize(Roles = "Participant")]
    public async Task<IActionResult> Enrol(int eventId, EnrolmentDto dto)
    {
        var userId = UserId();
        if (userId == null) return Unauthorized();

        var eventExists = await _db.Events.AnyAsync(
            x => x.EventID == eventId);

        if (!eventExists) return NotFound();

        var category = await _db.Categories.SingleOrDefaultAsync(
            x => x.CategoryID == dto.CategoryID &&
                 x.EventID == eventId);

        if (category == null)
            return BadRequest(new { message = "Invalid category for this event." });

        if (await _db.Enrolments.AnyAsync(
            x => x.ParticipantID == userId.Value &&
                 x.EventID == eventId))
        {
            return Conflict(new { message = "Already enrolled in this event." });
        }

        if (category.MaximumParticipants.HasValue)
        {
            var count = await _db.Enrolments.CountAsync(
                x => x.CategoryID == category.CategoryID &&
                     x.Status != "Cancelled");

            if (count >= category.MaximumParticipants.Value)
                return Conflict(new { message = "Category is full." });
        }

        var enrolment = new Enrolment
        {
            ParticipantID = userId.Value,
            EventID = eventId,
            CategoryID = category.CategoryID,
            EnrolmentDate = DateTime.UtcNow,
            Status = "Confirmed"
        };

        _db.Enrolments.Add(enrolment);
        await _db.SaveChangesAsync();

        return StatusCode(201, new
        {
            enrolment.EnrolmentID,
            enrolment.EventID,
            enrolment.CategoryID,
            enrolment.EnrolmentDate,
            enrolment.Status
        });
    }

    [HttpGet("api/enrolments/me")]
    [Authorize(Roles = "Participant")]
    public async Task<IActionResult> GetMine()
    {
        var userId = UserId();
        if (userId == null) return Unauthorized();

        var list = await _db.Enrolments.AsNoTracking()
            .Where(x => x.ParticipantID == userId.Value)
            .Select(x => new
            {
                x.EnrolmentID,
                x.EventID,
                EventName = x.Event!.EventName,
                x.CategoryID,
                CategoryName = x.Category!.CategoryName,
                x.EnrolmentDate,
                x.Status
            }).ToListAsync();

        return Ok(list);
    }

    [HttpGet("api/events/{eventId:int}/enrolments")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> GetForEvent(int eventId)
    {
        var userId = UserId();
        if (userId == null) return Unauthorized();

        bool owns = await _db.Events.AnyAsync(
            x => x.EventID == eventId && x.OrganiserID == userId.Value);

        if (!owns) return NotFound();

        var list = await _db.Enrolments.AsNoTracking()
            .Where(x => x.EventID == eventId)
            .Select(x => new
            {
                x.EnrolmentID,
                x.ParticipantID,
                ParticipantName = x.Participant!.FirstName + " " +
                                  x.Participant.LastName,
                x.CategoryID,
                CategoryName = x.Category!.CategoryName,
                x.EnrolmentDate,
                x.Status
            }).ToListAsync();

        return Ok(list);
    }

    [HttpDelete("api/enrolments/{enrolmentId:int}")]
    [Authorize]
    public async Task<IActionResult> Cancel(int enrolmentId)
    {
        var userId = UserId();
        if (userId == null) return Unauthorized();

        var enrolment = await _db.Enrolments
            .SingleOrDefaultAsync(x => x.EnrolmentID == enrolmentId);

        if (enrolment == null) return NotFound();

        bool isParticipant = enrolment.ParticipantID == userId.Value;

        bool isOrganiser = await _db.Events.AnyAsync(
            x => x.EventID == enrolment.EventID &&
                 x.OrganiserID == userId.Value);

        if (!isParticipant && !isOrganiser) return Forbid();

        if (enrolment.Status == "Cancelled")
            return Conflict(new { message = "Enrolment is already cancelled." });

        enrolment.Status = "Cancelled";
        await _db.SaveChangesAsync();

        return NoContent();
    }
}