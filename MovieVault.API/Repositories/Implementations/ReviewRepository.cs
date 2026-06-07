using Microsoft.EntityFrameworkCore;
using MovieVault.API.Data;
using MovieVault.API.Models.Domain;
using MovieVault.API.Repositories.Interfaces;

namespace MovieVault.API.Repositories.Implementations;

public class ReviewRepository : IReviewRepository
{
    private readonly MovieVaultDbContext _context;

    public ReviewRepository(MovieVaultDbContext context)
    {
        _context = context;
    }

    public async Task<List<Review>> GetByTmdbIdAsync(int tmdbId, int page, int pageSize)
        => await _context.Reviews
            .Where(r => r.TmdbId == tmdbId)
            .Include(r => r.User)
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

    public async Task<List<Review>> GetByUserIdAsync(Guid userId)
        => await _context.Reviews
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

    public async Task<Review?> GetByIdAsync(Guid id)
        => await _context.Reviews
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == id);

    public async Task<Review> CreateAsync(Review review)
    {
        await _context.Reviews.AddAsync(review);
        await _context.SaveChangesAsync();
        return review;
    }

    public async Task<Review> UpdateAsync(Review review)
    {
        _context.Reviews.Update(review);
        await _context.SaveChangesAsync();
        return review;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var review = await _context.Reviews.FindAsync(id);
        if (review is null) return false;

        _context.Reviews.Remove(review);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(Guid userId, int tmdbId)
        => await _context.Reviews
            .AnyAsync(r => r.UserId == userId && r.TmdbId == tmdbId);
}
