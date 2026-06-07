using MovieVault.API.Models.Domain;

namespace MovieVault.API.Repositories.Interfaces;

public interface IFavoriteRepository
{
    Task<List<Favorite>> GetByUserIdAsync(Guid userId, int page, int pageSize);
    Task<Favorite?> GetByIdAsync(Guid id);
    Task<Favorite> AddAsync(Favorite favorite);
    Task<bool> RemoveAsync(Guid id);
    Task<bool> ExistsAsync(Guid userId, int tmdbId);
    Task<bool> ClearAllAsync(Guid userId);
    Task<int> GetCountAsync(Guid userId);
}
