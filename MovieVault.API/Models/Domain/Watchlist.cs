namespace MovieVault.API.Models.Domain;

public class Watchlist
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public ICollection<WatchlistItem> Items { get; set; } = new List<WatchlistItem>();
}
