namespace MovieVault.API.Models.DTOs;

public class CreateWatchlistDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class UpdateWatchlistDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class WatchlistResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public Guid UserId { get; set; }
    public List<WatchlistItemResponseDto> Items { get; set; } = new();
}

public class AddWatchlistItemDto
{
    public int TmdbId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string PosterPath { get; set; } = string.Empty;
    public string MediaType { get; set; } = string.Empty;
}

public class WatchlistItemResponseDto
{
    public Guid Id { get; set; }
    public Guid WatchlistId { get; set; }
    public int TmdbId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string PosterPath { get; set; } = string.Empty;
    public string MediaType { get; set; } = string.Empty;
    public DateTime AddedAt { get; set; }
    public bool IsWatched { get; set; }
}
