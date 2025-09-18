using BusinessObjects;
using BusinessObjects.Dtos.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

namespace Services.Interface;

public interface IAuthService
{
    Task<AuthResponseDto> Login(LoginDto loginDto);

    Task<(User user, string? accessToken, string? refreshToken, string? tokenType)> GetUserWithTokens(
        HttpContext context);

    Task<AuthResponseDto> RefreshTokenAsync(string token);
    TokenValidationParameters GetValidationParameters();
}