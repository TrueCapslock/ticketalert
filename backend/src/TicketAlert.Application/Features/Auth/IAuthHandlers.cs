using TicketAlert.Application.Common.DTOs;

namespace TicketAlert.Application.Features.Auth;

public interface IRegisterUserHandler
{
    Task<AuthResponse> HandleAsync(RegisterRequest request);
}

public interface ILoginUserHandler
{
    Task<AuthResponse> HandleAsync(LoginRequest request);
}

public interface IRefreshTokenHandler
{
    Task<AuthResponse> HandleAsync(RefreshTokenRequest request);
}

public interface IForgotPasswordHandler
{
    Task HandleAsync(ForgotPasswordRequest request);
}

public interface IResetPasswordHandler
{
    Task HandleAsync(ResetPasswordRequest request);
}

public interface IGetCurrentUserHandler
{
    Task<UserDto> HandleAsync(Guid userId);
}
