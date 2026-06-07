using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieVault.API.Models.DTOs;
using MovieVault.API.Services.Interfaces;

namespace MovieVault.API.Controllers;

[ApiController]
[Route("api/reviews")]
public class ReviewController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    private Guid? GetUserId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(claim, out var id) ? id : null;
    }

    [HttpGet("movie/{tmdbId:int}")]
    public async Task<IActionResult> GetReviewsByTmdbId(
        int tmdbId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var reviews = await _reviewService.GetReviewsByTmdbIdAsync(tmdbId, page, pageSize);
        return Ok(reviews);
    }

    [HttpGet("user/{userId:guid}")]
    public async Task<IActionResult> GetReviewsByUserId(Guid userId)
    {
        var reviews = await _reviewService.GetReviewsByUserIdAsync(userId);
        return Ok(reviews);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetReviewById(Guid id)
    {
        var review = await _reviewService.GetReviewByIdAsync(id);
        return review is null ? NotFound() : Ok(review);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateReview([FromBody] CreateReviewDto dto)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        try
        {
            var review = await _reviewService.CreateReviewAsync(userId.Value, dto);
            return CreatedAtAction(nameof(GetReviewById), new { id = review.Id }, review);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateReview(Guid id, [FromBody] UpdateReviewDto dto)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var review = await _reviewService.UpdateReviewAsync(id, userId.Value, dto);
        return review is null ? NotFound() : Ok(review);
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteReview(Guid id)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var deleted = await _reviewService.DeleteReviewAsync(id, userId.Value);
        return deleted ? NoContent() : NotFound();
    }
}
