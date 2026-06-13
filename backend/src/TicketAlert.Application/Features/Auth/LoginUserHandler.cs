using Microsoft.EntityFrameworkCore;
using TicketAlert.Application.Common.DTOs;
using TicketAlert.Application.Common.Interfaces;

namespace TicketAlert.Application.Features.Auth;

public class LoginUserHandler : ILoginUserHandler
{
    private readonly IAppDbContext _db;
    private readonly IAuthService _auth;

    public LoginUserHandler(IAppDbContext db, IAuthService auth)
    {
        _db = db;
        _auth = auth;
    }

    public async Task<AuthResponse> HandleAsync(LoginRequest request)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email.ToLower());
        if (user is null || !_auth.VerifyPassword(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password.");

        var (token, refreshToken, expiresAt) = await _auth.GenerateTokensAsync(user);

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiresAt = expiresAt;
        user.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return new AuthResponse(token, refreshToken, expiresAt,
            new UserDto(user.Id, user.Email, user.Name, user.EmailVerified));
    }
}
