using AutoMapper;
using FluentAssertions;
using Moq;
using MovieVault.API.Mappings;
using MovieVault.API.Models.Domain;
using MovieVault.API.Models.DTOs;
using MovieVault.API.Repositories.Interfaces;
using MovieVault.API.Services.Implementations;

namespace MovieVault.Tests;

public class ReviewServiceTests
{
    private readonly Mock<IReviewRepository> _reviewRepositoryMock = new();
    private readonly IMapper _mapper;

    public ReviewServiceTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
        _mapper = config.CreateMapper();
    }

    private ReviewService CreateSut() =>
        new ReviewService(_reviewRepositoryMock.Object, _mapper);

    [Fact]
    public async Task CreateReviewAsync_Success_CallsCreateAsync()
    {
        var userId = Guid.NewGuid();
        var dto = new CreateReviewDto { TmdbId = 550, Title = "Fight Club", Content = "Great movie." };

        _reviewRepositoryMock.Setup(r => r.ExistsAsync(userId, dto.TmdbId)).ReturnsAsync(false);
        _reviewRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<Review>()))
            .ReturnsAsync((Review r) => r);

        var sut = CreateSut();

        var result = await sut.CreateReviewAsync(userId, dto);

        result.Should().NotBeNull();
        result.TmdbId.Should().Be(dto.TmdbId);
        result.Content.Should().Be(dto.Content);
        _reviewRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<Review>()), Times.Once);
    }

    [Fact]
    public async Task CreateReviewAsync_Duplicate_ThrowsInvalidOperationException()
    {
        var userId = Guid.NewGuid();
        var dto = new CreateReviewDto { TmdbId = 550, Title = "Fight Club", Content = "Good." };

        _reviewRepositoryMock.Setup(r => r.ExistsAsync(userId, dto.TmdbId)).ReturnsAsync(true);

        var sut = CreateSut();

        var act = async () => await sut.CreateReviewAsync(userId, dto);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*already*");
    }

    [Fact]
    public async Task UpdateReviewAsync_NotOwner_ReturnsNull()
    {
        var ownerId = Guid.NewGuid();
        var requesterId = Guid.NewGuid();
        var reviewId = Guid.NewGuid();

        _reviewRepositoryMock.Setup(r => r.GetByIdAsync(reviewId))
            .ReturnsAsync(new Review { Id = reviewId, UserId = ownerId, Content = "Original." });

        var sut = CreateSut();

        var result = await sut.UpdateReviewAsync(reviewId, requesterId, new UpdateReviewDto { Content = "Changed." });

        result.Should().BeNull();
        _reviewRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Review>()), Times.Never);
    }

    [Fact]
    public async Task DeleteReviewAsync_Success_CallsDeleteAsync()
    {
        var userId = Guid.NewGuid();
        var reviewId = Guid.NewGuid();

        _reviewRepositoryMock.Setup(r => r.GetByIdAsync(reviewId))
            .ReturnsAsync(new Review { Id = reviewId, UserId = userId });
        _reviewRepositoryMock.Setup(r => r.DeleteAsync(reviewId)).ReturnsAsync(true);

        var sut = CreateSut();

        var result = await sut.DeleteReviewAsync(reviewId, userId);

        result.Should().BeTrue();
        _reviewRepositoryMock.Verify(r => r.DeleteAsync(reviewId), Times.Once);
    }

    [Fact]
    public async Task GetReviewsByTmdbIdAsync_PassesCorrectPaginationParams()
    {
        var tmdbId = 550;
        var page = 2;
        var pageSize = 5;

        var reviews = new List<Review>
        {
            new() { Id = Guid.NewGuid(), TmdbId = tmdbId, UserId = Guid.NewGuid(),
                    Title = "Fight Club", Content = "Brilliant.", CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), TmdbId = tmdbId, UserId = Guid.NewGuid(),
                    Title = "Fight Club", Content = "Masterpiece.", CreatedAt = DateTime.UtcNow }
        };

        _reviewRepositoryMock
            .Setup(r => r.GetByTmdbIdAsync(tmdbId, page, pageSize))
            .ReturnsAsync(reviews);

        var sut = CreateSut();

        var result = await sut.GetReviewsByTmdbIdAsync(tmdbId, page, pageSize);

        _reviewRepositoryMock.Verify(r => r.GetByTmdbIdAsync(tmdbId, page, pageSize), Times.Once);
        result.Should().HaveCount(2);
    }
}
