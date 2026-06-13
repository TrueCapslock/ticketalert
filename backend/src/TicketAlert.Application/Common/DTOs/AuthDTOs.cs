using System.ComponentModel.DataAnnotations;

namespace TicketAlert.Application.Common.DTOs;

public record RegisterRequest(
    [Required][EmailAddress] string Email,
    [Required][MinLength(8)] string Password,
    string? Name
);

public record LoginRequest(
    [Required][EmailAddress] string Email,
    [Required] string Password
);

public record AuthResponse(
    string Token,
    string RefreshToken,
    DateTime ExpiresAt,
    UserDto User
);

public record RefreshTokenRequest(
    string RefreshToken
);

public record ForgotPasswordRequest(
    [Required][EmailAddress] string Email
);

public record ResetPasswordRequest(
    [Required] string Token,
    [Required][MinLength(8)] string Password
);

public record UserDto(
    Guid Id,
    string Email,
    string? Name,
    bool EmailVerified
);
