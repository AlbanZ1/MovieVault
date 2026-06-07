using AutoMapper;
using FluentAssertions;
using Moq;
using MovieVault.API.Mappings;
using MovieVault.API.Models.Domain;
using MovieVault.API.Models.DTOs;
using MovieVault.API.Repositories.Interfaces;
using MovieVault.API.Services.Implementations;

namespace MovieVault.Tests;

public class WatchlistServiceTests
{
    private readonly Mock<IWatchlistRepository> _watchlistRepositoryMock = new();
    private readonly IMapper _mapper;

    public WatchlistServiceTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
        _mapper = config.CreateMapper();
    }

    private WatchlistService CreateSut() =>
        new WatchlistService(_watchlistRepositoryMock.Object, _mapper);

    [Fact]
    public async Task GetUserWatchlistsAsync_ReturnsWatchlists_CountMatches()
    {
        var userId = Guid.NewGuid();
        var watchlists = new List<Watchlist>
        {
            new() { Id = Guid.NewGuid(), Name = "List A", UserId = userId },
            new() { Id = Guid.NewGuid(), Name = "List B", UserId = userId },
            new() { Id = Guid.NewGuid(), Name = "List C", UserId = userId }
        };

        _watchlistRepositoryMock
            .Setup(r => r.GetAllByUserIdAsync(userId))
            .ReturnsAsync(watchlists);

        var sut = CreateSut();

        var result = await sut.GetUserWatchlistsAsync(userId);

        result.Should().HaveCount(3);
        result.Select(w => w.Name).Should().BeEquivalentTo("List A", "List B", "List C");
    }

    [Fact]
    public async Task GetWatchlistByIdAsync_NotOwner_ReturnsNull()
    {
        var ownerId = Guid.NewGuid();
        var requesterId = Guid.NewGuid();
        var watchlistId = Guid.NewGuid();

        _watchlistRepositoryMock
            .Setup(r => r.GetByIdAsync(watchlistId))
            .ReturnsAsync(new Watchlist { Id = watchlistId, UserId = ownerId });

        var sut = CreateSut();

        var result = await sut.GetWatchlistByIdAsync(watchlistId, requesterId);

        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateWatchlistAsync_Success_CreatesWithCorrectUserIdAndName()
    {
        var userId = Guid.NewGuid();
        var dto = new CreateWatchlistDto { Name = "My Watchlist", Description = "A test list" };

        _watchlistRepositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<Watchlist>()))
            .ReturnsAsync((Watchlist w) => w);

        var sut = CreateSut();

        var result = await sut.CreateWatchlistAsync(userId, dto);

        result.Should().NotBeNull();
        result.Name.Should().Be(dto.Name);
        result.UserId.Should().Be(userId);
        _watchlistRepositoryMock.Verify(
            r => r.CreateAsync(It.Is<Watchlist>(w => w.UserId == userId && w.Name == dto.Name)),
            Times.Once);
    }

    [Fact]
    public async Task DeleteWatchlistAsync_NotOwner_ReturnsFalse()
    {
        var ownerId = Guid.NewGuid();
        var requesterId = Guid.NewGuid();
        var watchlistId = Guid.NewGuid();

        _watchlistRepositoryMock
            .Setup(r => r.GetByIdAsync(watchlistId))
            .ReturnsAsync(new Watchlist { Id = watchlistId, UserId = ownerId });

        var sut = CreateSut();

        var result = await sut.DeleteWatchlistAsync(watchlistId, requesterId);

        result.Should().BeFalse();
        _watchlistRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task ToggleWatchedStatusAsync_TogglesIsWatchedFromFalseToTrue()
    {
        var userId = Guid.NewGuid();
        var watchlistId = Guid.NewGuid();
        var itemId = Guid.NewGuid();

        var item = new WatchlistItem { Id = itemId, IsWatched = false };
        var watchlist = new Watchlist
        {
            Id = watchlistId,
            UserId = userId,
            Items = new List<WatchlistItem> { item }
        };

        _watchlistRepositoryMock
            .Setup(r => r.GetByIdAsync(watchlistId))
            .ReturnsAsync(watchlist);
        _watchlistRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Watchlist>()))
            .ReturnsAsync((Watchlist w) => w);

        var sut = CreateSut();

        var result = await sut.ToggleWatchedStatusAsync(watchlistId, itemId, userId);

        result.Should().BeTrue();
        item.IsWatched.Should().BeTrue();
        _watchlistRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Watchlist>()), Times.Once);
    }
}
