using MovieVault.API.Helpers;
using MovieVault.API.Models.Domain;
using MovieVault.API.Models.DTOs;
using MovieVault.API.Repositories.Interfaces;
using MovieVault.API.Services.Interfaces;

namespace MovieVault.API.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly JwtTokenGenerator _jwtTokenGenerator;

    public AuthService(IUserRepository userRepository, JwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto)
    {
        if (await _userRepository.GetByEmailAsync(dto.Email) is not null)
            throw new InvalidOperationException("Email is already in use.");

        if (await _userRepository.GetByUsernameAsync(dto.Username) is not null)
            throw new InvalidOperationException("Username is already taken.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.CreateAsync(user);

        return new AuthResponseDto
        {
            Token = _jwtTokenGenerator.GenerateToken(user),
            User = MapToUserDto(user)
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto)
    {
        var user = await _userRepository.GetByEmailAsync(dto.Email)
            ?? throw new UnauthorizedAccessException("Invalid email or password.");

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password.");

        return new AuthResponseDto
        {
            Token = _jwtTokenGenerator.GenerateToken(user),
            User = MapToUserDto(user)
        };
    }

    public async Task<UserDto> GetCurrentUserAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new KeyNotFoundException("User not found.");

        return MapToUserDto(user);
    }

    private static UserDto MapToUserDto(User user) => new()
    {
        Id = user.Id,
        Username = user.Username,
        Email = user.Email,
        CreatedAt = user.CreatedAt
    };
}
