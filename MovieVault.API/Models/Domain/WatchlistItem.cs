namespace MovieVault.API.Models.Domain;

public class WatchlistItem
{
    public Guid Id { get; set; }

    public Guid WatchlistId { get; set; }
    public Watchlist Watchlist { get; set; } = null!;

    public int TmdbId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string PosterPath { get; set; } = string.Empty;
    public string MediaType { get; set; } = string.Empty;
    public DateTime AddedAt { get; set; }
    public bool IsWatched { get; set; }
}
