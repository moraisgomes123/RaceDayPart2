using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.DTOs;
using RaceDay.Api.Models;
using System.Security.Claims;

namespace RaceDay.Api.Controllers;

[ApiController]
public class ResultsController : ControllerBase
{
    private readonly RaceDayContext _db;

    public ResultsController(RaceDayContext db) => _db = db;

    private int? UserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(value, out int id) ? id : null;
    }

    private static bool ValidStatus(string status) =>
        new[] { "Finished", "DNF", "DNS", "Disqualified" }
            .Contains(status, StringComparer.OrdinalIgnoreCase);

    [HttpPost("api/events/{eventId:int}/results")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Create(int eventId, ResultDto dto)
    {
        var userId = UserId();
        if (userId == null) return Unauthorized();

        bool ownsEvent = await _db.Events.AnyAsync(
            x => x.EventID == eventId &&
                 x.OrganiserID == userId.Value);

        if (!ownsEvent) return NotFound();

        var enrolment = await _db.Enrolments
            .SingleOrDefaultAsync(x =>
                x.EnrolmentID == dto.EnrolmentID &&
                x.EventID == eventId);

        if (enrolment == null)
            return BadRequest(new { message = "Invalid enrolment for this event." });

        if (await _db.Results.AnyAsync(
            x => x.EnrolmentID == dto.EnrolmentID))
        {
            return Conflict(new { message = "A result already exists." });
        }

        if (!ValidStatus(dto.ResultStatus))
            return BadRequest(new { message = "Invalid result status." });

        var result = new Result
        {
            EnrolmentID = dto.EnrolmentID,
            FinishPosition = dto.FinishPosition,
            FinishTime = dto.FinishTime,
            ResultStatus = dto.ResultStatus
        };

        _db.Results.Add(result);
        await _db.SaveChangesAsync();

        return StatusCode(201, new
        {
            result.ResultID,
            result.EnrolmentID,
            result.FinishPosition,
            result.FinishTime,
            result.ResultStatus
        });
    }

    [HttpGet("api/results/me")]
    [Authorize(Roles = "Participant")]
    public async Task<IActionResult> GetMine()
    {
        var userId = UserId();
        if (userId == null) return Unauthorized();

        var results = await _db.Results.AsNoTracking()
            .Where(x => x.Enrolment!.ParticipantID == userId.Value)
            .Select(x => new
            {
                x.ResultID,
                x.EnrolmentID,
                EventName = x.Enrolment!.Event!.EventName,
                x.FinishPosition,
                x.FinishTime,
                x.ResultStatus
            }).ToListAsync();

        return Ok(results);
    }

    [HttpGet("api/events/{eventId:int}/results")]
    public async Task<IActionResult> GetForEvent(int eventId)
    {
        if (!await _db.Events.AnyAsync(x => x.EventID == eventId))
            return NotFound();

        var results = await _db.Results.AsNoTracking()
            .Where(x => x.Enrolment!.EventID == eventId)
            .Select(x => new
            {
                x.ResultID,
                x.EnrolmentID,
                ParticipantName = x.Enrolment!.Participant!.FirstName +
                                  " " + x.Enrolment.Participant.LastName,
                x.FinishPosition,
                x.FinishTime,
                x.ResultStatus
            }).ToListAsync();

        return Ok(results);
    }

    [HttpPut("api/results/{resultId:int}")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Update(int resultId, ResultDto dto)
    {
        var userId = UserId();
        if (userId == null) return Unauthorized();

        var result = await _db.Results
            .Include(x => x.Enrolment)
            .ThenInclude(x => x!.Event)
            .SingleOrDefaultAsync(x => x.ResultID == resultId);

        if (result == null) return NotFound();

        if (result.Enrolment?.Event?.OrganiserID != userId.Value)
            return Forbid();

        if (result.EnrolmentID != dto.EnrolmentID)
            return BadRequest(new
            {
                message = "The enrolment ID cannot be changed."
            });

        if (!ValidStatus(dto.ResultStatus))
            return BadRequest(new { message = "Invalid result status." });

        result.FinishPosition = dto.FinishPosition;
        result.FinishTime = dto.FinishTime;
        result.ResultStatus = dto.ResultStatus;

        await _db.SaveChangesAsync();
        return Ok(new { message = "Result updated successfully." });
    }

    [HttpDelete("api/results/{resultId:int}")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Delete(int resultId)
    {
        var userId = UserId();
        if (userId == null) return Unauthorized();

        var result = await _db.Results
            .Include(x => x.Enrolment)
            .ThenInclude(x => x!.Event)
            .SingleOrDefaultAsync(x => x.ResultID == resultId);

        if (result == null) return NotFound();

        if (result.Enrolment?.Event?.OrganiserID != userId.Value)
            return Forbid();

        _db.Results.Remove(result);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}