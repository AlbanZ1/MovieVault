using Microsoft.EntityFrameworkCore;
using MovieVault.API.Data;
using MovieVault.API.Models.Domain;
using MovieVault.API.Repositories.Interfaces;

namespace MovieVault.API.Repositories.Implementations;

public class RatingRepository : IRatingRepository
{
    private readonly MovieVaultDbContext _context;

    public RatingRepository(MovieVaultDbContext context)
    {
        _context = context;
    }

    public async Task<Rating?> GetByUserAndTmdbIdAsync(Guid userId, int tmdbId)
        => await _context.Ratings
            .FirstOrDefaultAsync(r => r.UserId == userId && r.TmdbId == tmdbId);

    public async Task<List<Rating>> GetByTmdbIdAsync(int tmdbId)
        => await _context.Ratings
            .Where(r => r.TmdbId == tmdbId)
            .ToListAsync();

    public async Task<Rating?> GetByIdAsync(Guid id)
        => await _context.Ratings.FirstOrDefaultAsync(r => r.Id == id);

    public async Task<Rating> CreateAsync(Rating rating)
    {
        await _context.Ratings.AddAsync(rating);
        await _context.SaveChangesAsync();
        return rating;
    }

    public async Task<Rating> UpdateAsync(Rating rating)
    {
        _context.Ratings.Update(rating);
        await _context.SaveChangesAsync();
        return rating;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var rating = await _context.Ratings.FindAsync(id);
        if (rating is null) return false;

        _context.Ratings.Remove(rating);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<double> GetAverageRatingAsync(int tmdbId)
        => await _context.Ratings
            .Where(r => r.TmdbId == tmdbId)
            .Select(r => (double?)r.Score)
            .AverageAsync() ?? 0;
}
