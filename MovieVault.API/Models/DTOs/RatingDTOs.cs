namespace MovieVault.API.Models.DTOs;

public class CreateRatingDto
{
    public int TmdbId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Score { get; set; }
}

public class UpdateRatingDto
{
    public int Score { get; set; }
}

public class RatingResponseDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int TmdbId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Score { get; set; }
    public DateTime CreatedAt { get; set; }
}
