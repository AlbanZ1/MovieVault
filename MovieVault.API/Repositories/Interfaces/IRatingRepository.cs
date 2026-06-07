using MovieVault.API.Models.Domain;

namespace MovieVault.API.Repositories.Interfaces;

public interface IRatingRepository
{
    Task<Rating?> GetByUserAndTmdbIdAsync(Guid userId, int tmdbId);
    Task<List<Rating>> GetByTmdbIdAsync(int tmdbId);
    Task<Rating?> GetByIdAsync(Guid id);
    Task<Rating> CreateAsync(Rating rating);
    Task<Rating> UpdateAsync(Rating rating);
    Task<bool> DeleteAsync(Guid id);
    Task<double> GetAverageRatingAsync(int tmdbId);
}
