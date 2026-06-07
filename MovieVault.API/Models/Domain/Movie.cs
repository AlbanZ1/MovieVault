namespace MovieVault.API.Models.Domain;

public class Movie
{
    public int Id { get; set; }
    public int TmdbId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Overview { get; set; } = string.Empty;
    public string PosterPath { get; set; } = string.Empty;
    public string BackdropPath { get; set; } = string.Empty;
    public DateTime? ReleaseDate { get; set; }
    public double VoteAverage { get; set; }
    public string MediaType { get; set; } = string.Empty;
    public string GenreIds { get; set; } = string.Empty;

    public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
}
