using TicketAlert.Domain.Entities;

namespace TicketAlert.Application.Common.Interfaces;

public interface IAuthService
{
    Task<(string Token, string RefreshToken, DateTime ExpiresAt)> GenerateTokensAsync(User user);
    Task<string?> RefreshTokenAsync(string refreshToken);
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
    string GeneratePasswordResetToken();
}
