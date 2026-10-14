using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.DTOs;
using RaceDay.Api.Models;
using System.Security.Claims;

namespace RaceDay.Api.Controllers;

[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly RaceDayContext _db;

    public CategoriesController(RaceDayContext db) => _db = db;

    private int? UserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(value, out int id) ? id : null;
    }

    [HttpGet("api/categories")]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _db.Categories.AsNoTracking()
            .Select(x => new
            {
                x.CategoryID,
                x.EventID,
                x.CategoryName,
                x.DistanceKm,
                x.MaximumParticipants,
                x.EntryFee
            }).ToListAsync();

        return Ok(categories);
    }

    [HttpGet("api/events/{eventId:int}/categories")]
    public async Task<IActionResult> GetForEvent(int eventId)
    {
        if (!await _db.Events.AnyAsync(x => x.EventID == eventId))
            return NotFound();

        var categories = await _db.Categories.AsNoTracking()
            .Where(x => x.EventID == eventId)
            .Select(x => new
            {
                x.CategoryID,
                x.EventID,
                x.CategoryName,
                x.DistanceKm,
                x.MaximumParticipants,
                x.EntryFee
            }).ToListAsync();

        return Ok(categories);
    }

    [HttpPost("api/events/{eventId:int}/categories")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Create(int eventId, CategoryDto dto)
    {
        var userId = UserId();
        if (userId == null) return Unauthorized();

        bool owns = await _db.Events.AnyAsync(
            x => x.EventID == eventId && x.OrganiserID == userId.Value);

        if (!owns) return NotFound();

        if (await _db.Categories.AnyAsync(
            x => x.EventID == eventId &&
                 x.CategoryName == dto.CategoryName.Trim()))
        {
            return Conflict(new { message = "Category already exists." });
        }

        var category = new Category
        {
            EventID = eventId,
            CategoryName = dto.CategoryName.Trim(),
            DistanceKm = dto.DistanceKm,
            MaximumParticipants = dto.MaximumParticipants,
            EntryFee = dto.EntryFee
        };

        _db.Categories.Add(category);
        await _db.SaveChangesAsync();

        return StatusCode(201, new
        {
            category.CategoryID,
            category.EventID,
            category.CategoryName,
            category.EntryFee
        });
    }

    [HttpPut("api/categories/{categoryId:int}")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Update(
        int categoryId, CategoryDto dto)
    {
        var userId = UserId();
        if (userId == null) return Unauthorized();

        var category = await _db.Categories
            .Include(x => x.Event)
            .SingleOrDefaultAsync(x => x.CategoryID == categoryId);

        if (category == null) return NotFound();
        if (category.Event?.OrganiserID != userId.Value) return Forbid();

        bool duplicate = await _db.Categories.AnyAsync(
            x => x.EventID == category.EventID &&
                 x.CategoryID != categoryId &&
                 x.CategoryName == dto.CategoryName.Trim());

        if (duplicate)
            return Conflict(new { message = "Category name already exists." });

        category.CategoryName = dto.CategoryName.Trim();
        category.DistanceKm = dto.DistanceKm;
        category.MaximumParticipants = dto.MaximumParticipants;
        category.EntryFee = dto.EntryFee;

        await _db.SaveChangesAsync();
        return Ok(new { message = "Category updated successfully." });
    }

    [HttpDelete("api/categories/{categoryId:int}")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Delete(int categoryId)
    {
        var userId = UserId();
        if (userId == null) return Unauthorized();

        var category = await _db.Categories
            .Include(x => x.Event)
            .SingleOrDefaultAsync(x => x.CategoryID == categoryId);

        if (category == null) return NotFound();
        if (category.Event?.OrganiserID != userId.Value) return Forbid();

        if (await _db.Enrolments.AnyAsync(
            x => x.CategoryID == categoryId))
        {
            return Conflict(new
            {
                message = "Category has enrolments and cannot be deleted."
            });
        }

        _db.Categories.Remove(category);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}