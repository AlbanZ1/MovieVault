using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieVault.API.Models.DTOs;
using MovieVault.API.Services.Interfaces;

namespace MovieVault.API.Controllers;

[ApiController]
[Route("api/ratings")]
public class RatingController : ControllerBase
{
    private readonly IRatingService _ratingService;

    public RatingController(IRatingService ratingService)
    {
        _ratingService = ratingService;
    }

    private Guid? GetUserId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(claim, out var id) ? id : null;
    }

    [HttpGet("movie/{tmdbId:int}")]
    public async Task<IActionResult> GetRatingsByTmdbId(int tmdbId)
    {
        var ratings = await _ratingService.GetRatingsByTmdbIdAsync(tmdbId);
        return Ok(ratings);
    }

    [HttpGet("movie/{tmdbId:int}/average")]
    public async Task<IActionResult> GetAverageRating(int tmdbId)
    {
        var average = await _ratingService.GetAverageRatingAsync(tmdbId);
        return Ok(average);
    }

    [Authorize]
    [HttpGet("my/{tmdbId:int}")]
    public async Task<IActionResult> GetUserRating(int tmdbId)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var rating = await _ratingService.GetUserRatingAsync(userId.Value, tmdbId);
        return rating is null ? NotFound() : Ok(rating);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateRating([FromBody] CreateRatingDto dto)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        try
        {
            var rating = await _ratingService.CreateRatingAsync(userId.Value, dto);
            return CreatedAtAction(nameof(GetUserRating), new { tmdbId = rating.TmdbId }, rating);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateRating(Guid id, [FromBody] UpdateRatingDto dto)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var rating = await _ratingService.UpdateRatingAsync(id, userId.Value, dto);
        return rating is null ? NotFound() : Ok(rating);
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteRating(Guid id)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var deleted = await _ratingService.DeleteRatingAsync(id, userId.Value);
        return deleted ? NoContent() : NotFound();
    }
}
