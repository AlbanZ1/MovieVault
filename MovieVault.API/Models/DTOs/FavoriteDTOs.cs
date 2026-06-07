namespace MovieVault.API.Models.DTOs;

public class AddFavoriteDto
{
    public int TmdbId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string PosterPath { get; set; } = string.Empty;
    public string MediaType { get; set; } = string.Empty;
}

public class FavoriteResponseDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int TmdbId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string PosterPath { get; set; } = string.Empty;
    public string MediaType { get; set; } = string.Empty;
    public DateTime AddedAt { get; set; }
}
