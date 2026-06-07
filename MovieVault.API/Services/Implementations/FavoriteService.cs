using AutoMapper;
using MovieVault.API.Models.Domain;
using MovieVault.API.Models.DTOs;
using MovieVault.API.Repositories.Interfaces;
using MovieVault.API.Services.Interfaces;

namespace MovieVault.API.Services.Implementations;

public class FavoriteService : IFavoriteService
{
    private readonly IFavoriteRepository _favoriteRepository;
    private readonly IMapper _mapper;

    public FavoriteService(IFavoriteRepository favoriteRepository, IMapper mapper)
    {
        _favoriteRepository = favoriteRepository;
        _mapper = mapper;
    }

    public async Task<List<FavoriteResponseDto>> GetUserFavoritesAsync(Guid userId, int page, int pageSize)
    {
        var favorites = await _favoriteRepository.GetByUserIdAsync(userId, page, pageSize);
        return _mapper.Map<List<FavoriteResponseDto>>(favorites);
    }

    public async Task<FavoriteResponseDto> AddFavoriteAsync(Guid userId, AddFavoriteDto dto)
    {
        if (await _favoriteRepository.ExistsAsync(userId, dto.TmdbId))
            throw new InvalidOperationException("This title is already in your favorites.");

        var favorite = new Favorite
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TmdbId = dto.TmdbId,
            Title = dto.Title,
            PosterPath = dto.PosterPath,
            MediaType = dto.MediaType,
            AddedAt = DateTime.UtcNow
        };

        var added = await _favoriteRepository.AddAsync(favorite);
        return _mapper.Map<FavoriteResponseDto>(added);
    }

    public async Task<bool> RemoveFavoriteAsync(Guid favoriteId, Guid userId)
    {
        var favorite = await _favoriteRepository.GetByIdAsync(favoriteId);
        if (favorite is null || favorite.UserId != userId) return false;

        return await _favoriteRepository.RemoveAsync(favoriteId);
    }

    public async Task<bool> IsFavoriteAsync(Guid userId, int tmdbId)
        => await _favoriteRepository.ExistsAsync(userId, tmdbId);

    public async Task<bool> ClearAllFavoritesAsync(Guid userId)
        => await _favoriteRepository.ClearAllAsync(userId);

    public async Task<int> GetFavoritesCountAsync(Guid userId)
        => await _favoriteRepository.GetCountAsync(userId);
}
