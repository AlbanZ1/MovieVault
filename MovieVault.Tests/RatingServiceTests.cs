using AutoMapper;
using FluentAssertions;
using Moq;
using MovieVault.API.Mappings;
using MovieVault.API.Models.Domain;
using MovieVault.API.Models.DTOs;
using MovieVault.API.Repositories.Interfaces;
using MovieVault.API.Services.Implementations;

namespace MovieVault.Tests;

public class RatingServiceTests
{
    private readonly Mock<IRatingRepository> _ratingRepositoryMock = new();
    private readonly IMapper _mapper;

    public RatingServiceTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
        _mapper = config.CreateMapper();
    }

    private RatingService CreateSut() =>
        new RatingService(_ratingRepositoryMock.Object, _mapper);

    [Fact]
    public async Task CreateRatingAsync_Success_CallsCreateAsync()
    {
        var userId = Guid.NewGuid();
        var dto = new CreateRatingDto { TmdbId = 550, Title = "Fight Club", Score = 9 };

        _ratingRepositoryMock.Setup(r => r.GetByUserAndTmdbIdAsync(userId, dto.TmdbId))
            .ReturnsAsync((Rating?)null);
        _ratingRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<Rating>()))
            .ReturnsAsync((Rating r) => r);

        var sut = CreateSut();

        var result = await sut.CreateRatingAsync(userId, dto);

        result.Should().NotBeNull();
        result.Score.Should().Be(dto.Score);
        result.TmdbId.Should().Be(dto.TmdbId);
        _ratingRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<Rating>()), Times.Once);
    }

    [Fact]
    public async Task CreateRatingAsync_Duplicate_ThrowsInvalidOperationException()
    {
        var userId = Guid.NewGuid();
        var dto = new CreateRatingDto { TmdbId = 550, Title = "Fight Club", Score = 9 };

        _ratingRepositoryMock.Setup(r => r.GetByUserAndTmdbIdAsync(userId, dto.TmdbId))
            .ReturnsAsync(new Rating { UserId = userId, TmdbId = dto.TmdbId, Score = 7 });

        var sut = CreateSut();

        var act = async () => await sut.CreateRatingAsync(userId, dto);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*already*");
    }

    [Fact]
    public async Task UpdateRatingAsync_Success_UpdatesScoreAndCallsUpdateAsync()
    {
        var userId = Guid.NewGuid();
        var ratingId = Guid.NewGuid();
        var existing = new Rating
        {
            Id = ratingId,
            UserId = userId,
            TmdbId = 550,
            Title = "Fight Club",
            Score = 7,
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        };
        var dto = new UpdateRatingDto { Score = 10 };

        _ratingRepositoryMock.Setup(r => r.GetByIdAsync(ratingId)).ReturnsAsync(existing);
        _ratingRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Rating>()))
            .ReturnsAsync((Rating r) => r);

        var sut = CreateSut();

        var result = await sut.UpdateRatingAsync(ratingId, userId, dto);

        result.Should().NotBeNull();
        result!.Score.Should().Be(dto.Score);
        _ratingRepositoryMock.Verify(
            r => r.UpdateAsync(It.Is<Rating>(r => r.Score == dto.Score)),
            Times.Once);
    }

    [Fact]
    public async Task GetAverageRatingAsync_ReturnsCorrectAverage()
    {
        var tmdbId = 550;
        var expectedAverage = 8.5;

        _ratingRepositoryMock.Setup(r => r.GetAverageRatingAsync(tmdbId)).ReturnsAsync(expectedAverage);

        var sut = CreateSut();

        var result = await sut.GetAverageRatingAsync(tmdbId);

        result.Should().Be(expectedAverage);
    }

    [Fact]
    public async Task DeleteRatingAsync_NotOwner_ReturnsFalse()
    {
        var ownerId = Guid.NewGuid();
        var requesterId = Guid.NewGuid();
        var ratingId = Guid.NewGuid();

        _ratingRepositoryMock.Setup(r => r.GetByIdAsync(ratingId))
            .ReturnsAsync(new Rating { Id = ratingId, UserId = ownerId, Score = 8 });

        var sut = CreateSut();

        var result = await sut.DeleteRatingAsync(ratingId, requesterId);

        result.Should().BeFalse();
        _ratingRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Guid>()), Times.Never);
    }
}
