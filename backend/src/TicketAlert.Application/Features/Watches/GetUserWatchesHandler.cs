using Microsoft.EntityFrameworkCore;
using TicketAlert.Application.Common.DTOs;
using TicketAlert.Application.Common.Interfaces;
using TicketAlert.Domain.Enums;

namespace TicketAlert.Application.Features.Watches;

public class GetUserWatchesHandler : IGetUserWatchesHandler
{
    private readonly IAppDbContext _db;

    public GetUserWatchesHandler(IAppDbContext db) => _db = db;

    public async Task<IReadOnlyList<WatchDto>> HandleAsync(Guid userId, string? status)
    {
        var query = _db.Watches
            .Include(w => w.Event)
            .Where(w => w.UserId == userId);

        if (!string.IsNullOrEmpty(status) && Enum.TryParse<WatchStatus>(status, true, out var parsed))
            query = query.Where(w => w.Status == parsed);

        return await query
            .OrderByDescending(w => w.CreatedAt)
            .Select(w => new WatchDto(
                w.Id, w.EventId, w.Event.Title, w.Event.Artist,
                w.Event.Venue, w.Event.EventDate, w.Event.TicketmasterUrl,
                w.Status.ToString(), w.CreatedAt, w.ExpiresAt, w.TriggeredAt))
            .ToListAsync();
    }
}
