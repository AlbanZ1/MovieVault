using MovieVault.API.Models.Domain;

namespace MovieVault.API.Repositories.Interfaces;

public interface IReviewRepository
{
    Task<List<Review>> GetByTmdbIdAsync(int tmdbId, int page, int pageSize);
    Task<List<Review>> GetByUserIdAsync(Guid userId);
    Task<Review?> GetByIdAsync(Guid id);
    Task<Review> CreateAsync(Review review);
    Task<Review> UpdateAsync(Review review);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid userId, int tmdbId);
}
