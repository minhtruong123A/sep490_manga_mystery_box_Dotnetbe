using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BusinessObjects;
using BusinessObjects.Dtos.Auth;
using DataAccessLayers.Exceptions;
using DataAccessLayers.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Services.Interface;

namespace Services.Service;

public class AuthService(IUnitOfWork unitOfWork, IConfiguration configuration) : IAuthService
{
    public async Task<AuthResponseDto> Login(LoginDto loginDto)
    {
        var account = await unitOfWork.UserRepository.GetSystemAccountByAccountName(loginDto.Name);
        if (account == null || !VerifyPassword(loginDto.Password, account.Password ?? ""))
            throw new UnauthorizedAccessException("Wrong email or password.");

        //if (!account.IsActive)
        //    throw new UnauthorizedAccessException("Inactive account.");

        var accessToken = CreateToken(account, false, 60);
        var refreshToken = CreateToken(account, true, 60 * 24 * 7);

        return new AuthResponseDto
        {
            Token = accessToken,
            RefreshToken = refreshToken,
            TokenType = "Bearer",
            Username = account.Username,
            Email = account.Email,
            Role = account.RoleId,
            IsEmailVerification = account.EmailVerification
        };
    }

    public async Task<(User user, string? accessToken, string? refreshToken, string? tokenType)> GetUserWithTokens(
        HttpContext context)
    {
        var claims = context.User;
        var userName = claims.FindFirst(c => c.Type == "username")?.Value
                       ?? throw new Exception("User not found.");
        var account = await unitOfWork.UserRepository.GetSystemAccountByAccountName(userName)
                      ?? throw new Exception("User not found.");
        if (account.IsActive is false || account.EmailVerification is false)
            throw new ForbiddenException("Forbidden: Account is inactive or email not verified.");
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        string? accessToken = null,
            tokenType = null;

        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            tokenType = "Bearer";
            accessToken = authHeader.Substring("Bearer ".Length).Trim();
        }

        var refreshToken = context.Request.Headers["X-Refresh-Token"].FirstOrDefault();

        return (account, accessToken, refreshToken, tokenType);
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token)) throw new ArgumentException("Refresh token is required");

        var handler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:JWT_SECRET"]));
        try
        {
            var principal = handler.ValidateToken(token, GetValidationParameters(), out _);
            var claims = principal.Claims.ToDictionary(c => c.Type, c => c.Value);

            if (!claims.TryGetValue("is_refresh_token", out var isRefresh) || isRefresh != "true")
                throw new SecurityTokenException("Provided token is not a refresh token");
            if (!claims.TryGetValue("username", out var username) || string.IsNullOrEmpty(username))
                throw new SecurityTokenException("Invalid token: username missing");

            var user = await unitOfWork.UserRepository.GetSystemAccountByAccountName(username)
                       ?? throw new Exception("User not found");
            var accessToken = CreateToken(user, false, 60);
            var refreshToken = CreateToken(user, true, 60 * 24 * 7);

            return new AuthResponseDto
            {
                Token = accessToken,
                RefreshToken = refreshToken,
                TokenType = "Bearer",
                Username = username,
                Email = user.Email,
                Role = claims.GetValueOrDefault("role"),
                IsEmailVerification = user.EmailVerification
            };
        }
        catch (SecurityTokenExpiredException)
        {
            throw new UnauthorizedAccessException("Refresh token has expired");
        }
        catch (Exception ex) when (ex is SecurityTokenException || ex is ArgumentException)
        {
            throw new UnauthorizedAccessException($"Invalid refresh token: {ex.Message}");
        }
    }

    public TokenValidationParameters GetValidationParameters()
    {
        return new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            //RequireExpirationTime = true,
            IssuerSigningKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:JWT_SECRET"]))
        };
    }


    private bool VerifyPassword(string enteredPassword, string storedHash)
    {
        return BCrypt.Net.BCrypt.Verify(enteredPassword, storedHash);
    }

    private string CreateToken(User account, bool isRefreshToken = false, int expireMinutes = 30)
    {
        var rawKey = configuration["JwtSettings:JWT_SECRET"];
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(rawKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiresAt = DateTime.UtcNow.AddMinutes(expireMinutes);

        var claims = new List<Claim>
        {
            new("username", account.Username ?? "unknown"),
            new("role", account.RoleId ?? ""),
            new("is_refresh_token", isRefreshToken ? "true" : "false")
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: expiresAt,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}