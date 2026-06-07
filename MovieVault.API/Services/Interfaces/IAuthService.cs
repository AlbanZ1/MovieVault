using MovieVault.API.Models.DTOs;

namespace MovieVault.API.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto);
    Task<AuthResponseDto> LoginAsync(LoginRequestDto dto);
    Task<UserDto> GetCurrentUserAsync(Guid userId);
}
