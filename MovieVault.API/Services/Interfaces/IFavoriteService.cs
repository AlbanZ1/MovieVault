using MovieVault.API.Models.DTOs;

namespace MovieVault.API.Services.Interfaces;

public interface IFavoriteService
{
    Task<List<FavoriteResponseDto>> GetUserFavoritesAsync(Guid userId, int page, int pageSize);
    Task<FavoriteResponseDto> AddFavoriteAsync(Guid userId, AddFavoriteDto dto);
    Task<bool> RemoveFavoriteAsync(Guid favoriteId, Guid userId);
    Task<bool> IsFavoriteAsync(Guid userId, int tmdbId);
    Task<bool> ClearAllFavoritesAsync(Guid userId);
    Task<int> GetFavoritesCountAsync(Guid userId);
}
