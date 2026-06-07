using Microsoft.EntityFrameworkCore;
using MovieVault.API.Data;
using MovieVault.API.Models.Domain;
using MovieVault.API.Repositories.Interfaces;

namespace MovieVault.API.Repositories.Implementations;

public class WatchlistRepository : IWatchlistRepository
{
    private readonly MovieVaultDbContext _context;

    public WatchlistRepository(MovieVaultDbContext context)
    {
        _context = context;
    }

    public async Task<List<Watchlist>> GetAllByUserIdAsync(Guid userId)
        => await _context.Watchlists
            .Where(w => w.UserId == userId)
            .Include(w => w.Items)
            .ToListAsync();

    public async Task<Watchlist?> GetByIdAsync(Guid id)
        => await _context.Watchlists
            .Include(w => w.Items)
            .FirstOrDefaultAsync(w => w.Id == id);

    public async Task<Watchlist> CreateAsync(Watchlist watchlist)
    {
        await _context.Watchlists.AddAsync(watchlist);
        await _context.SaveChangesAsync();
        return watchlist;
    }

    public async Task<Watchlist> UpdateAsync(Watchlist watchlist)
    {
        _context.Watchlists.Update(watchlist);
        await _context.SaveChangesAsync();
        return watchlist;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var watchlist = await _context.Watchlists.FindAsync(id);
        if (watchlist is null) return false;

        _context.Watchlists.Remove(watchlist);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<WatchlistItem> AddItemAsync(WatchlistItem item)
    {
        await _context.WatchlistItems.AddAsync(item);
        await _context.SaveChangesAsync();
        return item;
    }

    public async Task<bool> RemoveItemAsync(Guid itemId)
    {
        var item = await _context.WatchlistItems.FindAsync(itemId);
        if (item is null) return false;

        _context.WatchlistItems.Remove(item);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<WatchlistItem>> GetItemsByWatchlistIdAsync(Guid watchlistId)
        => await _context.WatchlistItems
            .Where(wi => wi.WatchlistId == watchlistId)
            .ToListAsync();

    public async Task<bool> ItemExistsAsync(Guid watchlistId, int tmdbId)
        => await _context.WatchlistItems
            .AnyAsync(wi => wi.WatchlistId == watchlistId && wi.TmdbId == tmdbId);
}
