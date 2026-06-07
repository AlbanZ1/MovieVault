using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using MovieVault.API.Helpers;
using MovieVault.API.Models.Domain;
using MovieVault.API.Models.DTOs;
using MovieVault.API.Repositories.Interfaces;
using MovieVault.API.Services.Implementations;

namespace MovieVault.Tests;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock = new();

    private AuthService CreateSut()
    {
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(c => c["JwtSettings:Key"]).Returns("TestSecretKey_AtLeast32CharsLong!!");
        configMock.Setup(c => c["JwtSettings:Issuer"]).Returns("TestIssuer");
        configMock.Setup(c => c["JwtSettings:Audience"]).Returns("TestAudience");
        var jwtGenerator = new JwtTokenGenerator(configMock.Object);
        return new AuthService(_userRepositoryMock.Object, jwtGenerator);
    }

    [Fact]
    public async Task RegisterAsync_Success_ReturnsAuthResponseWithToken()
    {
        var dto = new RegisterRequestDto
        {
            Username = "alice",
            Email = "alice@test.com",
            Password = "password123"
        };

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync((User?)null);
        _userRepositoryMock.Setup(r => r.GetByUsernameAsync(dto.Username)).ReturnsAsync((User?)null);
        _userRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<User>()))
            .ReturnsAsync((User u) => u);

        var sut = CreateSut();

        var result = await sut.RegisterAsync(dto);

        result.Should().NotBeNull();
        result.Token.Should().NotBeNullOrEmpty();
        result.User.Email.Should().Be(dto.Email);
        result.User.Username.Should().Be(dto.Username);
        _userRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_DuplicateEmail_ThrowsInvalidOperationException()
    {
        var dto = new RegisterRequestDto
        {
            Username = "alice",
            Email = "existing@test.com",
            Password = "password123"
        };

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(dto.Email))
            .ReturnsAsync(new User { Email = dto.Email });

        var sut = CreateSut();

        var act = async () => await sut.RegisterAsync(dto);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Email*");
    }

    [Fact]
    public async Task RegisterAsync_DuplicateUsername_ThrowsInvalidOperationException()
    {
        var dto = new RegisterRequestDto
        {
            Username = "existinguser",
            Email = "new@test.com",
            Password = "password123"
        };

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync((User?)null);
        _userRepositoryMock.Setup(r => r.GetByUsernameAsync(dto.Username))
            .ReturnsAsync(new User { Username = dto.Username });

        var sut = CreateSut();

        var act = async () => await sut.RegisterAsync(dto);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Username*");
    }

    [Fact]
    public async Task LoginAsync_Success_ReturnsAuthResponseWithToken()
    {
        var password = "password123";
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "alice@test.com",
            Username = "alice",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            CreatedAt = DateTime.UtcNow
        };

        var dto = new LoginRequestDto { Email = user.Email, Password = password };

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync(user);

        var sut = CreateSut();

        var result = await sut.LoginAsync(dto);

        result.Should().NotBeNull();
        result.Token.Should().NotBeNullOrEmpty();
        result.User.Email.Should().Be(user.Email);
    }

    [Fact]
    public async Task LoginAsync_InvalidEmail_ThrowsUnauthorizedAccessException()
    {
        var dto = new LoginRequestDto { Email = "ghost@test.com", Password = "password123" };

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync((User?)null);

        var sut = CreateSut();

        var act = async () => await sut.LoginAsync(dto);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task LoginAsync_InvalidPassword_ThrowsUnauthorizedAccessException()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "alice@test.com",
            Username = "alice",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("correctpassword"),
            CreatedAt = DateTime.UtcNow
        };

        var dto = new LoginRequestDto { Email = user.Email, Password = "wrongpassword" };

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync(user);

        var sut = CreateSut();

        var act = async () => await sut.LoginAsync(dto);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }
}
