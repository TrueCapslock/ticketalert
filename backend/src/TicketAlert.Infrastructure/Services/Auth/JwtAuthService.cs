using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TicketAlert.Application.Common.Interfaces;
using TicketAlert.Domain.Entities;

namespace TicketAlert.Infrastructure.Services.Auth;

public class JwtAuthService : IAuthService
{
    private readonly IConfiguration _config;

    public JwtAuthService(IConfiguration config) => _config = config;

    public Task<(string Token, string RefreshToken, DateTime ExpiresAt)> GenerateTokensAsync(User user)
    {
        var expiresAt = DateTime.UtcNow.AddHours(1);
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Name ?? user.Email)
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: creds);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        return Task.FromResult((tokenString, refreshToken, expiresAt));
    }

    public async Task<string?> RefreshTokenAsync(string refreshToken)
    {
        // Validate refresh token against DB — implemented in handler
        return await Task.FromResult<string?>(null);
    }

    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }

    public string GeneratePasswordResetToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
    }
}
