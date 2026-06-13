using Microsoft.EntityFrameworkCore;
using TicketAlert.Application.Common.DTOs;
using TicketAlert.Application.Common.Interfaces;
using TicketAlert.Domain.Entities;

namespace TicketAlert.Application.Features.Auth;

public class RegisterUserHandler : IRegisterUserHandler
{
    private readonly IAppDbContext _db;
    private readonly IAuthService _auth;

    public RegisterUserHandler(IAppDbContext db, IAuthService auth)
    {
        _db = db;
        _auth = auth;
    }

    public async Task<AuthResponse> HandleAsync(RegisterRequest request)
    {
        if (await _db.Users.AnyAsync(u => u.Email == request.Email.ToLower()))
            throw new InvalidOperationException("Email already registered.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email.ToLower().Trim(),
            PasswordHash = _auth.HashPassword(request.Password),
            Name = request.Name?.Trim()
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var (token, refreshToken, expiresAt) = await _auth.GenerateTokensAsync(user);

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiresAt = expiresAt;
        await _db.SaveChangesAsync();

        return new AuthResponse(token, refreshToken, expiresAt,
            new UserDto(user.Id, user.Email, user.Name, user.EmailVerified));
    }
}
