using AutoMapper;
using MovieVault.API.Models.Domain;
using MovieVault.API.Models.DTOs;
using MovieVault.API.Repositories.Interfaces;
using MovieVault.API.Services.Interfaces;

namespace MovieVault.API.Services.Implementations;

public class RatingService : IRatingService
{
    private readonly IRatingRepository _ratingRepository;
    private readonly IMapper _mapper;

    public RatingService(IRatingRepository ratingRepository, IMapper mapper)
    {
        _ratingRepository = ratingRepository;
        _mapper = mapper;
    }

    public async Task<RatingResponseDto?> GetUserRatingAsync(Guid userId, int tmdbId)
    {
        var rating = await _ratingRepository.GetByUserAndTmdbIdAsync(userId, tmdbId);
        return rating is null ? null : _mapper.Map<RatingResponseDto>(rating);
    }

    public async Task<List<RatingResponseDto>> GetRatingsByTmdbIdAsync(int tmdbId)
    {
        var ratings = await _ratingRepository.GetByTmdbIdAsync(tmdbId);
        return _mapper.Map<List<RatingResponseDto>>(ratings);
    }

    public async Task<RatingResponseDto> CreateRatingAsync(Guid userId, CreateRatingDto dto)
    {
        if (await _ratingRepository.GetByUserAndTmdbIdAsync(userId, dto.TmdbId) is not null)
            throw new InvalidOperationException("You have already rated this title.");

        var rating = new Rating
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TmdbId = dto.TmdbId,
            Title = dto.Title,
            Score = dto.Score,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _ratingRepository.CreateAsync(rating);
        return _mapper.Map<RatingResponseDto>(created);
    }

    public async Task<RatingResponseDto?> UpdateRatingAsync(Guid id, Guid userId, UpdateRatingDto dto)
    {
        var rating = await _ratingRepository.GetByIdAsync(id);
        if (rating is null || rating.UserId != userId) return null;

        rating.Score = dto.Score;
        rating.CreatedAt = DateTime.UtcNow;

        var updated = await _ratingRepository.UpdateAsync(rating);
        return _mapper.Map<RatingResponseDto>(updated);
    }

    public async Task<bool> DeleteRatingAsync(Guid id, Guid userId)
    {
        var rating = await _ratingRepository.GetByIdAsync(id);
        if (rating is null || rating.UserId != userId) return false;

        return await _ratingRepository.DeleteAsync(id);
    }

    public async Task<double> GetAverageRatingAsync(int tmdbId)
        => await _ratingRepository.GetAverageRatingAsync(tmdbId);
}
