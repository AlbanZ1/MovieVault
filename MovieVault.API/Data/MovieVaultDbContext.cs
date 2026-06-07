using Microsoft.EntityFrameworkCore;
using MovieVault.API.Models.Domain;

namespace MovieVault.API.Data;

public class MovieVaultDbContext : DbContext
{
    public MovieVaultDbContext(DbContextOptions<MovieVaultDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Movie> Movies { get; set; }
    public DbSet<Watchlist> Watchlists { get; set; }
    public DbSet<WatchlistItem> WatchlistItems { get; set; }
    public DbSet<Favorite> Favorites { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Rating> Ratings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Watchlists)
            .WithOne(w => w.User)
            .HasForeignKey(w => w.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Favorites)
            .WithOne(f => f.User)
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Reviews)
            .WithOne(r => r.User)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Ratings)
            .WithOne(r => r.User)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Watchlist>()
            .HasMany(w => w.Items)
            .WithOne(wi => wi.Watchlist)
            .HasForeignKey(wi => wi.WatchlistId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Rating>()
            .ToTable(t => t.HasCheckConstraint("CK_Rating_Score", "[Score] >= 1 AND [Score] <= 10"));

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        // Movie.Favorites/Reviews/Ratings are not FK-backed (linked via TmdbId only)
        modelBuilder.Entity<Movie>()
            .Ignore(m => m.Favorites)
            .Ignore(m => m.Reviews)
            .Ignore(m => m.Ratings);
    }
}
