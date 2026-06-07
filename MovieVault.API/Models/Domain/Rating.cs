namespace MovieVault.API.Models.Domain;

public class Rating
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public int TmdbId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Score { get; set; }
    public DateTime CreatedAt { get; set; }
}
