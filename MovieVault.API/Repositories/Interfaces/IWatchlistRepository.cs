using MovieVault.API.Models.Domain;

namespace MovieVault.API.Repositories.Interfaces;

public interface IWatchlistRepository
{
    Task<List<Watchlist>> GetAllByUserIdAsync(Guid userId);
    Task<Watchlist?> GetByIdAsync(Guid id);
    Task<Watchlist> CreateAsync(Watchlist watchlist);
    Task<Watchlist> UpdateAsync(Watchlist watchlist);
    Task<bool> DeleteAsync(Guid id);
    Task<WatchlistItem> AddItemAsync(WatchlistItem item);
    Task<bool> RemoveItemAsync(Guid itemId);
    Task<List<WatchlistItem>> GetItemsByWatchlistIdAsync(Guid watchlistId);
    Task<bool> ItemExistsAsync(Guid watchlistId, int tmdbId);
}
