using Microsoft.AspNetCore.Mvc;
using MovieVault.API.Integrations.TMDB;

namespace MovieVault.API.Controllers;

[ApiController]
[Route("api/movies")]
public class MovieController : ControllerBase
{
    private readonly ITmdbService _tmdbService;

    public MovieController(ITmdbService tmdbService)
    {
        _tmdbService = tmdbService;
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string query = "",
        [FromQuery] int page = 1,
        [FromQuery] string mediaType = "multi")
    {
        if (string.IsNullOrWhiteSpace(query))
            return BadRequest(new { message = "Query cannot be empty." });

        var results = await _tmdbService.SearchAsync(query, page, mediaType);
        return results.Results.Count == 0 ? NotFound() : Ok(results);
    }

    [HttpGet("popular")]
    public async Task<IActionResult> GetPopular(
        [FromQuery] string mediaType = "movie",
        [FromQuery] int page = 1)
    {
        var results = await _tmdbService.GetPopularAsync(mediaType, page);
        return results.Results.Count == 0 ? NotFound() : Ok(results);
    }

    [HttpGet("trending")]
    public async Task<IActionResult> GetTrending(
        [FromQuery] string mediaType = "movie",
        [FromQuery] string timeWindow = "week",
        [FromQuery] int page = 1)
    {
        var results = await _tmdbService.GetTrendingAsync(mediaType, timeWindow, page);
        return results.Results.Count == 0 ? NotFound() : Ok(results);
    }

    [HttpGet("genre/{genreId:int}")]
    public async Task<IActionResult> GetByGenre(
        int genreId,
        [FromQuery] string mediaType = "movie",
        [FromQuery] int page = 1)
    {
        var results = await _tmdbService.GetByGenreAsync(genreId, mediaType, page);
        return results.Results.Count == 0 ? NotFound() : Ok(results);
    }

    [HttpGet("movie/{tmdbId:int}")]
    public async Task<IActionResult> GetMovieDetail(int tmdbId)
    {
        var result = await _tmdbService.GetMovieDetailAsync(tmdbId);
        return result.Id == 0 ? NotFound() : Ok(result);
    }

    [HttpGet("tv/{tmdbId:int}")]
    public async Task<IActionResult> GetTvDetail(int tmdbId)
    {
        var result = await _tmdbService.GetTvDetailAsync(tmdbId);
        return result.Id == 0 ? NotFound() : Ok(result);
    }
}
