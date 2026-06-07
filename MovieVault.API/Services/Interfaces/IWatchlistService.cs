using MovieVault.API.Models.DTOs;

namespace MovieVault.API.Services.Interfaces;

public interface IWatchlistService
{
    Task<List<WatchlistResponseDto>> GetUserWatchlistsAsync(Guid userId);
    Task<WatchlistResponseDto?> GetWatchlistByIdAsync(Guid id, Guid userId);
    Task<WatchlistResponseDto> CreateWatchlistAsync(Guid userId, CreateWatchlistDto dto);
    Task<WatchlistResponseDto?> UpdateWatchlistAsync(Guid id, Guid userId, UpdateWatchlistDto dto);
    Task<bool> DeleteWatchlistAsync(Guid id, Guid userId);
    Task<WatchlistItemResponseDto> AddItemToWatchlistAsync(Guid watchlistId, Guid userId, AddWatchlistItemDto dto);
    Task<bool> RemoveItemFromWatchlistAsync(Guid watchlistId, Guid itemId, Guid userId);
    Task<List<WatchlistItemResponseDto>> GetWatchlistItemsAsync(Guid watchlistId, Guid userId);
    Task<bool> ToggleWatchedStatusAsync(Guid watchlistId, Guid itemId, Guid userId);
}
