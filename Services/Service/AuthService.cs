using BusinessObjects;
using BusinessObjects.Dtos.Auth;
using DataAccessLayers.Interface;
using DataAccessLayers.UnitOfWork;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _uniUnitOfWork;

        public AuthService(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _uniUnitOfWork = unitOfWork;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> Login(LoginDto loginDto)
        {
            var account = await _uniUnitOfWork.UserRepository.GetSystemAccountByAccountName(loginDto.Name);

            if (account == null || !VerifyPassword(loginDto.Password, account.Password ?? ""))
            {
                throw new UnauthorizedAccessException("Wrong email or password.");
            }

            var token = CreateToken(account);
            return new AuthResponseDto { Token = token };
        }

        private bool VerifyPassword(string enteredPassword, string storedHash)
        {
            return BCrypt.Net.BCrypt.Verify(enteredPassword, storedHash);
        }

        private string CreateToken(User account)
        {
            var rawKey = _configuration["JwtSettings:JWT_SECRET"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(rawKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiresAt = DateTime.UtcNow.AddMinutes(30);

            var claims = new List<Claim>
            {
                new("username", account.Username ?? "unknown"),
                new("role", account.RoleId ?? ""),
                new("is_refresh_token", "false")
            };


            var token = new JwtSecurityToken(
                claims: claims,
                expires: expiresAt,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<User> GetUserByClaims(ClaimsPrincipal claims)
        {
            var userName = claims.FindFirst(c => c.Type == "username")?.Value ?? throw new Exception("User not found.");
            var account = await _uniUnitOfWork.UserRepository.GetSystemAccountByAccountName(userName);

            return account ?? throw new Exception("User not found.");
        }
    }
}
