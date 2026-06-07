namespace MovieVault.API.Models.Domain;

public class Favorite
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public int TmdbId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string PosterPath { get; set; } = string.Empty;
    public string MediaType { get; set; } = string.Empty;
    public DateTime AddedAt { get; set; }
}
