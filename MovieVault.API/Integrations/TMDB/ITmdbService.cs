using MovieVault.API.Models.DTOs;

namespace MovieVault.API.Integrations.TMDB;

public interface ITmdbService
{
    Task<TmdbSearchResponseDto> SearchAsync(string query, int page, string mediaType);
    Task<MovieDetailDto> GetMovieDetailAsync(int tmdbId);
    Task<MovieDetailDto> GetTvDetailAsync(int tmdbId);
    Task<TmdbSearchResponseDto> GetPopularAsync(string mediaType, int page);
    Task<TmdbSearchResponseDto> GetTrendingAsync(string mediaType, string timeWindow, int page);
    Task<TmdbSearchResponseDto> GetByGenreAsync(int genreId, string mediaType, int page);
}
