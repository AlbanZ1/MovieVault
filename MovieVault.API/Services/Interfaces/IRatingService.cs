using MovieVault.API.Models.DTOs;

namespace MovieVault.API.Services.Interfaces;

public interface IRatingService
{
    Task<RatingResponseDto?> GetUserRatingAsync(Guid userId, int tmdbId);
    Task<List<RatingResponseDto>> GetRatingsByTmdbIdAsync(int tmdbId);
    Task<RatingResponseDto> CreateRatingAsync(Guid userId, CreateRatingDto dto);
    Task<RatingResponseDto?> UpdateRatingAsync(Guid id, Guid userId, UpdateRatingDto dto);
    Task<bool> DeleteRatingAsync(Guid id, Guid userId);
    Task<double> GetAverageRatingAsync(int tmdbId);
}
