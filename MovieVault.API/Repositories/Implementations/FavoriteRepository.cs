using Microsoft.EntityFrameworkCore;
using MovieVault.API.Data;
using MovieVault.API.Models.Domain;
using MovieVault.API.Repositories.Interfaces;

namespace MovieVault.API.Repositories.Implementations;

public class FavoriteRepository : IFavoriteRepository
{
    private readonly MovieVaultDbContext _context;

    public FavoriteRepository(MovieVaultDbContext context)
    {
        _context = context;
    }

    public async Task<List<Favorite>> GetByUserIdAsync(Guid userId, int page, int pageSize)
        => await _context.Favorites
            .Where(f => f.UserId == userId)
            .OrderByDescending(f => f.AddedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

    public async Task<Favorite?> GetByIdAsync(Guid id)
        => await _context.Favorites.FirstOrDefaultAsync(f => f.Id == id);

    public async Task<Favorite> AddAsync(Favorite favorite)
    {
        await _context.Favorites.AddAsync(favorite);
        await _context.SaveChangesAsync();
        return favorite;
    }

    public async Task<bool> RemoveAsync(Guid id)
    {
        var favorite = await _context.Favorites.FindAsync(id);
        if (favorite is null) return false;

        _context.Favorites.Remove(favorite);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(Guid userId, int tmdbId)
        => await _context.Favorites
            .AnyAsync(f => f.UserId == userId && f.TmdbId == tmdbId);

    public async Task<bool> ClearAllAsync(Guid userId)
    {
        var favorites = await _context.Favorites
            .Where(f => f.UserId == userId)
            .ToListAsync();

        if (favorites.Count == 0) return false;

        _context.Favorites.RemoveRange(favorites);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<int> GetCountAsync(Guid userId)
        => await _context.Favorites.CountAsync(f => f.UserId == userId);
}
