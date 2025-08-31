using Application.DTOs;

namespace Application.Common.Interfaces;

public interface IAuthService
{
    Task<Result> RegisterAsync(RegisterDto registerDto, string role);
    Task<AuthResult> LoginAsync(LoginDto loginDto);
    Task<AuthResult> RefreshTokenAsync(string refreshToken);
    Task<bool> RevokeTokenAsync(string refreshToken, string? revokeReason = null);
    Task<UserDto> GetUserInfoAsync(int userId);
    Task<AuthResult> LoginWithExternalProviderAsync(int userId);
    Task<AuthResult> HandleGoogleAuthCallbackAsync(string googleId, string email, string name);
}
