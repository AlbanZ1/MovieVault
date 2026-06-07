using AutoMapper;
using FluentAssertions;
using Moq;
using MovieVault.API.Mappings;
using MovieVault.API.Models.Domain;
using MovieVault.API.Models.DTOs;
using MovieVault.API.Repositories.Interfaces;
using MovieVault.API.Services.Implementations;

namespace MovieVault.Tests;

public class FavoriteServiceTests
{
    private readonly Mock<IFavoriteRepository> _favoriteRepositoryMock = new();
    private readonly IMapper _mapper;

    public FavoriteServiceTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
        _mapper = config.CreateMapper();
    }

    private FavoriteService CreateSut() =>
        new FavoriteService(_favoriteRepositoryMock.Object, _mapper);

    [Fact]
    public async Task AddFavoriteAsync_Success_CallsAddAsync()
    {
        var userId = Guid.NewGuid();
        var dto = new AddFavoriteDto { TmdbId = 550, Title = "Fight Club", PosterPath = "/poster.jpg", MediaType = "movie" };

        _favoriteRepositoryMock.Setup(r => r.ExistsAsync(userId, dto.TmdbId)).ReturnsAsync(false);
        _favoriteRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Favorite>()))
            .ReturnsAsync((Favorite f) => f);

        var sut = CreateSut();

        var result = await sut.AddFavoriteAsync(userId, dto);

        result.Should().NotBeNull();
        result.TmdbId.Should().Be(dto.TmdbId);
        result.UserId.Should().Be(userId);
        _favoriteRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Favorite>()), Times.Once);
    }

    [Fact]
    public async Task AddFavoriteAsync_AlreadyFavorited_ThrowsInvalidOperationException()
    {
        var userId = Guid.NewGuid();
        var dto = new AddFavoriteDto { TmdbId = 550, Title = "Fight Club", PosterPath = "/poster.jpg", MediaType = "movie" };

        _favoriteRepositoryMock.Setup(r => r.ExistsAsync(userId, dto.TmdbId)).ReturnsAsync(true);

        var sut = CreateSut();

        var act = async () => await sut.AddFavoriteAsync(userId, dto);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*already*");
    }

    [Fact]
    public async Task RemoveFavoriteAsync_NotOwner_ReturnsFalse()
    {
        var ownerId = Guid.NewGuid();
        var requesterId = Guid.NewGuid();
        var favoriteId = Guid.NewGuid();

        _favoriteRepositoryMock.Setup(r => r.GetByIdAsync(favoriteId))
            .ReturnsAsync(new Favorite { Id = favoriteId, UserId = ownerId });

        var sut = CreateSut();

        var result = await sut.RemoveFavoriteAsync(favoriteId, requesterId);

        result.Should().BeFalse();
        _favoriteRepositoryMock.Verify(r => r.RemoveAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task GetFavoritesCountAsync_ReturnsCorrectCount()
    {
        var userId = Guid.NewGuid();
        var expectedCount = 7;

        _favoriteRepositoryMock.Setup(r => r.GetCountAsync(userId)).ReturnsAsync(expectedCount);

        var sut = CreateSut();

        var result = await sut.GetFavoritesCountAsync(userId);

        result.Should().Be(expectedCount);
    }
}
