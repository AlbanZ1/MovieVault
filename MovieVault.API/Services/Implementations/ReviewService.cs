using AutoMapper;
using MovieVault.API.Models.Domain;
using MovieVault.API.Models.DTOs;
using MovieVault.API.Repositories.Interfaces;
using MovieVault.API.Services.Interfaces;

namespace MovieVault.API.Services.Implementations;

public class ReviewService : IReviewService
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IMapper _mapper;

    public ReviewService(IReviewRepository reviewRepository, IMapper mapper)
    {
        _reviewRepository = reviewRepository;
        _mapper = mapper;
    }

    public async Task<List<ReviewResponseDto>> GetReviewsByTmdbIdAsync(int tmdbId, int page, int pageSize)
    {
        var reviews = await _reviewRepository.GetByTmdbIdAsync(tmdbId, page, pageSize);
        return _mapper.Map<List<ReviewResponseDto>>(reviews);
    }

    public async Task<List<ReviewResponseDto>> GetReviewsByUserIdAsync(Guid userId)
    {
        var reviews = await _reviewRepository.GetByUserIdAsync(userId);
        return _mapper.Map<List<ReviewResponseDto>>(reviews);
    }

    public async Task<ReviewResponseDto?> GetReviewByIdAsync(Guid id)
    {
        var review = await _reviewRepository.GetByIdAsync(id);
        return review is null ? null : _mapper.Map<ReviewResponseDto>(review);
    }

    public async Task<ReviewResponseDto> CreateReviewAsync(Guid userId, CreateReviewDto dto)
    {
        if (await _reviewRepository.ExistsAsync(userId, dto.TmdbId))
            throw new InvalidOperationException("You have already reviewed this title.");

        var review = new Review
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TmdbId = dto.TmdbId,
            Title = dto.Title,
            Content = dto.Content,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _reviewRepository.CreateAsync(review);
        return _mapper.Map<ReviewResponseDto>(created);
    }

    public async Task<ReviewResponseDto?> UpdateReviewAsync(Guid id, Guid userId, UpdateReviewDto dto)
    {
        var review = await _reviewRepository.GetByIdAsync(id);
        if (review is null || review.UserId != userId) return null;

        review.Content = dto.Content;
        review.UpdatedAt = DateTime.UtcNow;

        var updated = await _reviewRepository.UpdateAsync(review);
        return _mapper.Map<ReviewResponseDto>(updated);
    }

    public async Task<bool> DeleteReviewAsync(Guid id, Guid userId)
    {
        var review = await _reviewRepository.GetByIdAsync(id);
        if (review is null || review.UserId != userId) return false;

        return await _reviewRepository.DeleteAsync(id);
    }
}
