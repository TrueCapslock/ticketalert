using Microsoft.EntityFrameworkCore;
using TicketAlert.Application.Common.DTOs;
using TicketAlert.Application.Common.Interfaces;

namespace TicketAlert.Application.Features.Auth;

public class GetCurrentUserHandler : IGetCurrentUserHandler
{
    private readonly IAppDbContext _db;

    public GetCurrentUserHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<UserDto> HandleAsync(Guid userId)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId)
            ?? throw new KeyNotFoundException("User not found.");

        return new UserDto(user.Id, user.Email, user.Name, user.EmailVerified);
    }
}
