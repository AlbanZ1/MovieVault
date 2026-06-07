using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieVault.API.Models.DTOs;
using MovieVault.API.Services.Interfaces;

namespace MovieVault.API.Controllers;

[ApiController]
[Route("api/watchlists")]
[Authorize]
public class WatchlistController : ControllerBase
{
    private readonly IWatchlistService _watchlistService;

    public WatchlistController(IWatchlistService watchlistService)
    {
        _watchlistService = watchlistService;
    }

    private Guid? GetUserId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(claim, out var id) ? id : null;
    }

    [HttpGet]
    public async Task<IActionResult> GetUserWatchlists()
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var watchlists = await _watchlistService.GetUserWatchlistsAsync(userId.Value);
        return Ok(watchlists);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetWatchlistById(Guid id)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var watchlist = await _watchlistService.GetWatchlistByIdAsync(id, userId.Value);
        return watchlist is null ? NotFound() : Ok(watchlist);
    }

    [HttpPost]
    public async Task<IActionResult> CreateWatchlist([FromBody] CreateWatchlistDto dto)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var watchlist = await _watchlistService.CreateWatchlistAsync(userId.Value, dto);
        return CreatedAtAction(nameof(GetWatchlistById), new { id = watchlist.Id }, watchlist);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateWatchlist(Guid id, [FromBody] UpdateWatchlistDto dto)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var watchlist = await _watchlistService.UpdateWatchlistAsync(id, userId.Value, dto);
        return watchlist is null ? NotFound() : Ok(watchlist);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteWatchlist(Guid id)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var deleted = await _watchlistService.DeleteWatchlistAsync(id, userId.Value);
        return deleted ? NoContent() : NotFound();
    }

    [HttpGet("{id:guid}/items")]
    public async Task<IActionResult> GetWatchlistItems(Guid id)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var items = await _watchlistService.GetWatchlistItemsAsync(id, userId.Value);
        return Ok(items);
    }

    [HttpPost("{id:guid}/items")]
    public async Task<IActionResult> AddItemToWatchlist(Guid id, [FromBody] AddWatchlistItemDto dto)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        try
        {
            var item = await _watchlistService.AddItemToWatchlistAsync(id, userId.Value, dto);
            return CreatedAtAction(nameof(GetWatchlistItems), new { id }, item);
        }
        catch (UnauthorizedAccessException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id:guid}/items/{itemId:guid}")]
    public async Task<IActionResult> RemoveItemFromWatchlist(Guid id, Guid itemId)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var removed = await _watchlistService.RemoveItemFromWatchlistAsync(id, itemId, userId.Value);
        return removed ? NoContent() : NotFound();
    }

    [HttpPatch("{id:guid}/items/{itemId:guid}/toggle")]
    public async Task<IActionResult> ToggleWatchedStatus(Guid id, Guid itemId)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var toggled = await _watchlistService.ToggleWatchedStatusAsync(id, itemId, userId.Value);
        return toggled ? NoContent() : NotFound();
    }
}
