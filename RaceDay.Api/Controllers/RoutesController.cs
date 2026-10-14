using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.DTOs;
using RaceDay.Api.Models;
using System.Security.Claims;
using RouteModel = RaceDay.Api.Models.Route;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api/events/{eventId:int}/routes")]
public class RoutesController : ControllerBase
{
    private readonly RaceDayContext _db;

    public RoutesController(RaceDayContext db) => _db = db;

    private int? UserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(value, out int id) ? id : null;
    }

    private async Task<bool> OwnsEvent(int eventId, int userId)
    {
        return await _db.Events.AnyAsync(
            x => x.EventID == eventId && x.OrganiserID == userId);
    }

    [HttpGet]
    public async Task<IActionResult> Get(int eventId)
    {
        if (!await _db.Events.AnyAsync(x => x.EventID == eventId))
            return NotFound();

        var route = await _db.Routes.AsNoTracking()
            .Where(x => x.EventID == eventId)
            .Select(x => new
            {
                x.RouteID,
                x.EventID,
                x.RouteName,
                x.DistanceKm,
                x.StartPoint,
                x.FinishPoint,
                x.RouteInformation
            })
            .SingleOrDefaultAsync();

        return route == null ? NotFound() : Ok(route);
    }

    [HttpPost]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Create(int eventId, RouteDto dto)
    {
        var userId = UserId();
        if (userId == null) return Unauthorized();

        if (!await OwnsEvent(eventId, userId.Value)) return NotFound();

        if (await _db.Routes.AnyAsync(x => x.EventID == eventId))
            return Conflict(new { message = "This event already has a route." });

        var route = new RouteModel
        {
            EventID = eventId,
            RouteName = dto.RouteName.Trim(),
            DistanceKm = dto.DistanceKm,
            StartPoint = dto.StartPoint.Trim(),
            FinishPoint = dto.FinishPoint.Trim(),
            RouteInformation = dto.RouteInformation
        };

        _db.Routes.Add(route);
        await _db.SaveChangesAsync();

        return StatusCode(201, new
        {
            route.RouteID,
            route.EventID,
            route.RouteName,
            route.DistanceKm,
            route.StartPoint,
            route.FinishPoint
        });
    }

    [HttpPut("{routeId:int}")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Update(
        int eventId, int routeId, RouteDto dto)
    {
        var userId = UserId();
        if (userId == null) return Unauthorized();

        if (!await OwnsEvent(eventId, userId.Value)) return NotFound();

        var route = await _db.Routes.SingleOrDefaultAsync(
            x => x.RouteID == routeId && x.EventID == eventId);

        if (route == null) return NotFound();

        route.RouteName = dto.RouteName.Trim();
        route.DistanceKm = dto.DistanceKm;
        route.StartPoint = dto.StartPoint.Trim();
        route.FinishPoint = dto.FinishPoint.Trim();
        route.RouteInformation = dto.RouteInformation;

        await _db.SaveChangesAsync();
        return Ok(new { message = "Route updated successfully." });
    }

    [HttpDelete("{routeId:int}")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Delete(int eventId, int routeId)
    {
        var userId = UserId();
        if (userId == null) return Unauthorized();

        if (!await OwnsEvent(eventId, userId.Value)) return NotFound();

        var route = await _db.Routes.SingleOrDefaultAsync(
            x => x.RouteID == routeId && x.EventID == eventId);

        if (route == null) return NotFound();

        _db.Routes.Remove(route);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}