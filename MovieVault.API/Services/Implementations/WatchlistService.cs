using AutoMapper;
using MovieVault.API.Models.Domain;
using MovieVault.API.Models.DTOs;
using MovieVault.API.Repositories.Interfaces;
using MovieVault.API.Services.Interfaces;

namespace MovieVault.API.Services.Implementations;

public class WatchlistService : IWatchlistService
{
    private readonly IWatchlistRepository _watchlistRepository;
    private readonly IMapper _mapper;

    public WatchlistService(IWatchlistRepository watchlistRepository, IMapper mapper)
    {
        _watchlistRepository = watchlistRepository;
        _mapper = mapper;
    }

    public async Task<List<WatchlistResponseDto>> GetUserWatchlistsAsync(Guid userId)
    {
        var watchlists = await _watchlistRepository.GetAllByUserIdAsync(userId);
        return _mapper.Map<List<WatchlistResponseDto>>(watchlists);
    }

    public async Task<WatchlistResponseDto?> GetWatchlistByIdAsync(Guid id, Guid userId)
    {
        var watchlist = await _watchlistRepository.GetByIdAsync(id);
        if (watchlist is null || watchlist.UserId != userId) return null;

        return _mapper.Map<WatchlistResponseDto>(watchlist);
    }

    public async Task<WatchlistResponseDto> CreateWatchlistAsync(Guid userId, CreateWatchlistDto dto)
    {
        var watchlist = new Watchlist
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _watchlistRepository.CreateAsync(watchlist);
        return _mapper.Map<WatchlistResponseDto>(created);
    }

    public async Task<WatchlistResponseDto?> UpdateWatchlistAsync(Guid id, Guid userId, UpdateWatchlistDto dto)
    {
        var watchlist = await _watchlistRepository.GetByIdAsync(id);
        if (watchlist is null || watchlist.UserId != userId) return null;

        watchlist.Name = dto.Name;
        watchlist.Description = dto.Description;

        var updated = await _watchlistRepository.UpdateAsync(watchlist);
        return _mapper.Map<WatchlistResponseDto>(updated);
    }

    public async Task<bool> DeleteWatchlistAsync(Guid id, Guid userId)
    {
        var watchlist = await _watchlistRepository.GetByIdAsync(id);
        if (watchlist is null || watchlist.UserId != userId) return false;

        return await _watchlistRepository.DeleteAsync(id);
    }

    public async Task<WatchlistItemResponseDto> AddItemToWatchlistAsync(
        Guid watchlistId, Guid userId, AddWatchlistItemDto dto)
    {
        var watchlist = await _watchlistRepository.GetByIdAsync(watchlistId);
        if (watchlist is null || watchlist.UserId != userId)
            throw new UnauthorizedAccessException("Watchlist not found or access denied.");

        var item = new WatchlistItem
        {
            Id = Guid.NewGuid(),
            WatchlistId = watchlistId,
            TmdbId = dto.TmdbId,
            Title = dto.Title,
            PosterPath = dto.PosterPath,
            MediaType = dto.MediaType,
            AddedAt = DateTime.UtcNow,
            IsWatched = false
        };

        var added = await _watchlistRepository.AddItemAsync(item);
        return _mapper.Map<WatchlistItemResponseDto>(added);
    }

    public async Task<bool> RemoveItemFromWatchlistAsync(Guid watchlistId, Guid itemId, Guid userId)
    {
        var watchlist = await _watchlistRepository.GetByIdAsync(watchlistId);
        if (watchlist is null || watchlist.UserId != userId) return false;

        return await _watchlistRepository.RemoveItemAsync(itemId);
    }

    public async Task<List<WatchlistItemResponseDto>> GetWatchlistItemsAsync(Guid watchlistId, Guid userId)
    {
        var watchlist = await _watchlistRepository.GetByIdAsync(watchlistId);
        if (watchlist is null || watchlist.UserId != userId) return [];

        var items = await _watchlistRepository.GetItemsByWatchlistIdAsync(watchlistId);
        return _mapper.Map<List<WatchlistItemResponseDto>>(items);
    }

    public async Task<bool> ToggleWatchedStatusAsync(Guid watchlistId, Guid itemId, Guid userId)
    {
        var watchlist = await _watchlistRepository.GetByIdAsync(watchlistId);
        if (watchlist is null || watchlist.UserId != userId) return false;

        var item = watchlist.Items.FirstOrDefault(i => i.Id == itemId);
        if (item is null) return false;

        // Entity is already tracked from the Include; flip the flag and persist
        item.IsWatched = !item.IsWatched;
        await _watchlistRepository.UpdateAsync(watchlist);
        return true;
    }
}
