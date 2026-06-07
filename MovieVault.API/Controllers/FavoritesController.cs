using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieVault.API.Models.DTOs;
using MovieVault.API.Services.Interfaces;

namespace MovieVault.API.Controllers;

[ApiController]
[Route("api/favorites")]
[Authorize]
public class FavoritesController : ControllerBase
{
    private readonly IFavoriteService _favoriteService;

    public FavoritesController(IFavoriteService favoriteService)
    {
        _favoriteService = favoriteService;
    }

    private Guid? GetUserId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(claim, out var id) ? id : null;
    }

    [HttpGet]
    public async Task<IActionResult> GetUserFavorites(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var favorites = await _favoriteService.GetUserFavoritesAsync(userId.Value, page, pageSize);
        return Ok(favorites);
    }

    [HttpPost]
    public async Task<IActionResult> AddFavorite([FromBody] AddFavoriteDto dto)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        try
        {
            var favorite = await _favoriteService.AddFavoriteAsync(userId.Value, dto);
            return CreatedAtAction(nameof(GetUserFavorites), favorite);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> RemoveFavorite(Guid id)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var removed = await _favoriteService.RemoveFavoriteAsync(id, userId.Value);
        return removed ? NoContent() : NotFound();
    }

    [HttpGet("check/{tmdbId:int}")]
    public async Task<IActionResult> CheckIsFavorite(int tmdbId)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var isFavorite = await _favoriteService.IsFavoriteAsync(userId.Value, tmdbId);
        return Ok(isFavorite);
    }

    [HttpDelete("clear")]
    public async Task<IActionResult> ClearAllFavorites()
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        await _favoriteService.ClearAllFavoritesAsync(userId.Value);
        return NoContent();
    }

    [HttpGet("count")]
    public async Task<IActionResult> GetFavoritesCount()
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var count = await _favoriteService.GetFavoritesCountAsync(userId.Value);
        return Ok(count);
    }
}
