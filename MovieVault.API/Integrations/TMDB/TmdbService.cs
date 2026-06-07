using System.Text.Json;
using MovieVault.API.Models.DTOs;

namespace MovieVault.API.Integrations.TMDB;

public class TmdbService : ITmdbService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _apiKey;
    private readonly string _baseUrl;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    public TmdbService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _apiKey = configuration["TmdbSettings:ApiKey"] ?? string.Empty;
        _baseUrl = configuration["TmdbSettings:BaseUrl"] ?? "https://api.themoviedb.org/3";
    }

    public async Task<TmdbSearchResponseDto> SearchAsync(string query, int page, string mediaType)
    {
        var encoded = Uri.EscapeDataString(query);
        var endpoint = mediaType.ToLowerInvariant() switch
        {
            "movie" => $"/search/movie?query={encoded}&page={page}",
            "tv"    => $"/search/tv?query={encoded}&page={page}",
            _       => $"/search/multi?query={encoded}&page={page}"
        };

        var raw = await GetAsync<TmdbListResponse>(endpoint);
        return MapToSearchResponse(raw, mediaType);
    }

    public async Task<MovieDetailDto> GetMovieDetailAsync(int tmdbId)
    {
        var raw = await GetAsync<TmdbDetailResponse>($"/movie/{tmdbId}");
        return MapToDetailDto(raw, "movie");
    }

    public async Task<MovieDetailDto> GetTvDetailAsync(int tmdbId)
    {
        var raw = await GetAsync<TmdbDetailResponse>($"/tv/{tmdbId}");
        return MapToDetailDto(raw, "tv");
    }

    public async Task<TmdbSearchResponseDto> GetPopularAsync(string mediaType, int page)
    {
        var type = mediaType.ToLowerInvariant() == "tv" ? "tv" : "movie";
        var raw = await GetAsync<TmdbListResponse>($"/{type}/popular?page={page}");
        return MapToSearchResponse(raw, type);
    }

    public async Task<TmdbSearchResponseDto> GetTrendingAsync(string mediaType, string timeWindow, int page)
    {
        var type = mediaType.ToLowerInvariant();
        var window = timeWindow.ToLowerInvariant() == "day" ? "day" : "week";
        var raw = await GetAsync<TmdbListResponse>($"/trending/{type}/{window}?page={page}");
        return MapToSearchResponse(raw, type);
    }

    public async Task<TmdbSearchResponseDto> GetByGenreAsync(int genreId, string mediaType, int page)
    {
        var type = mediaType.ToLowerInvariant() == "tv" ? "tv" : "movie";
        var raw = await GetAsync<TmdbListResponse>($"/discover/{type}?with_genres={genreId}&page={page}");
        return MapToSearchResponse(raw, type);
    }

    // -------------------------------------------------------------------------

    private async Task<T?> GetAsync<T>(string endpoint)
    {
        var client = _httpClientFactory.CreateClient();
        var separator = endpoint.Contains('?') ? "&" : "?";
        var url = $"{_baseUrl}{endpoint}{separator}api_key={_apiKey}";

        var response = await client.GetAsync(url);
        if (!response.IsSuccessStatusCode) return default;

        var stream = await response.Content.ReadAsStreamAsync();
        return await JsonSerializer.DeserializeAsync<T>(stream, JsonOptions);
    }

    private static TmdbSearchResponseDto MapToSearchResponse(TmdbListResponse? raw, string fallbackMediaType)
    {
        if (raw is null) return new TmdbSearchResponseDto();

        return new TmdbSearchResponseDto
        {
            Page = raw.Page,
            TotalPages = raw.TotalPages,
            TotalResults = raw.TotalResults,
            Results = raw.Results.Select(r => new MovieSearchResultDto
            {
                Id = r.Id,
                Title = r.Title ?? r.Name ?? string.Empty,
                Overview = r.Overview ?? string.Empty,
                PosterPath = r.PosterPath ?? string.Empty,
                BackdropPath = r.BackdropPath ?? string.Empty,
                ReleaseDate = r.ReleaseDate ?? r.FirstAirDate,
                VoteAverage = r.VoteAverage,
                MediaType = r.MediaType ?? fallbackMediaType,
                GenreIds = r.GenreIds ?? []
            }).ToList()
        };
    }

    private static MovieDetailDto MapToDetailDto(TmdbDetailResponse? raw, string mediaType)
    {
        if (raw is null) return new MovieDetailDto();

        return new MovieDetailDto
        {
            Id = raw.Id,
            Title = raw.Title ?? raw.Name ?? string.Empty,
            Overview = raw.Overview ?? string.Empty,
            PosterPath = raw.PosterPath ?? string.Empty,
            BackdropPath = raw.BackdropPath ?? string.Empty,
            ReleaseDate = raw.ReleaseDate ?? raw.FirstAirDate,
            VoteAverage = raw.VoteAverage,
            MediaType = mediaType,
            GenreIds = raw.Genres?.Select(g => g.Id).ToList() ?? []
        };
    }
}

// ── Internal TMDB deserialization models ──────────────────────────────────────

internal class TmdbListResponse
{
    public int Page { get; set; }
    public List<TmdbResultItem> Results { get; set; } = [];
    public int TotalPages { get; set; }
    public int TotalResults { get; set; }
}

internal class TmdbResultItem
{
    public int Id { get; set; }
    public string? Title { get; set; }       // movies
    public string? Name { get; set; }        // TV shows
    public string? Overview { get; set; }
    public string? PosterPath { get; set; }
    public string? BackdropPath { get; set; }
    public string? ReleaseDate { get; set; }    // movies
    public string? FirstAirDate { get; set; }   // TV shows
    public double VoteAverage { get; set; }
    public string? MediaType { get; set; }
    public List<int>? GenreIds { get; set; }
}

internal class TmdbDetailResponse
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Name { get; set; }
    public string? Overview { get; set; }
    public string? PosterPath { get; set; }
    public string? BackdropPath { get; set; }
    public string? ReleaseDate { get; set; }
    public string? FirstAirDate { get; set; }
    public double VoteAverage { get; set; }
    public List<TmdbGenreItem>? Genres { get; set; }
}

internal class TmdbGenreItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
