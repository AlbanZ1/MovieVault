using MovieVault.API.Models.DTOs;

namespace MovieVault.API.Services.Interfaces;

public interface IReviewService
{
    Task<List<ReviewResponseDto>> GetReviewsByTmdbIdAsync(int tmdbId, int page, int pageSize);
    Task<List<ReviewResponseDto>> GetReviewsByUserIdAsync(Guid userId);
    Task<ReviewResponseDto?> GetReviewByIdAsync(Guid id);
    Task<ReviewResponseDto> CreateReviewAsync(Guid userId, CreateReviewDto dto);
    Task<ReviewResponseDto?> UpdateReviewAsync(Guid id, Guid userId, UpdateReviewDto dto);
    Task<bool> DeleteReviewAsync(Guid id, Guid userId);
}
