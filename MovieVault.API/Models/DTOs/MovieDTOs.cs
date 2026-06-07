namespace MovieVault.API.Models.DTOs;

public class MovieSearchResultDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Overview { get; set; } = string.Empty;
    public string PosterPath { get; set; } = string.Empty;
    public string BackdropPath { get; set; } = string.Empty;
    public string? ReleaseDate { get; set; }
    public double VoteAverage { get; set; }
    public string MediaType { get; set; } = string.Empty;
    public List<int> GenreIds { get; set; } = new();
}

public class MovieDetailDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Overview { get; set; } = string.Empty;
    public string PosterPath { get; set; } = string.Empty;
    public string BackdropPath { get; set; } = string.Empty;
    public string? ReleaseDate { get; set; }
    public double VoteAverage { get; set; }
    public string MediaType { get; set; } = string.Empty;
    public List<int> GenreIds { get; set; } = new();
}

public class TmdbSearchResponseDto
{
    public int Page { get; set; }
    public List<MovieSearchResultDto> Results { get; set; } = new();
    public int TotalPages { get; set; }
    public int TotalResults { get; set; }
}
