using Microsoft.EntityFrameworkCore;
using TicketAlert.Application.Common.DTOs;
using TicketAlert.Application.Common.Interfaces;

namespace TicketAlert.Application.Features.Events;

public class GetEventHandler : IGetEventHandler
{
    private readonly IAppDbContext _db;

    public GetEventHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<EventDetailDto?> HandleAsync(Guid id)
    {
        var ev = await _db.Events
            .Include(e => e.Watches)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (ev is null) return null;

        return MapToDetail(ev);
    }

    public async Task<EventDetailDto?> HandleByTicketmasterIdAsync(string ticketmasterEventId)
    {
        var ev = await _db.Events
            .Include(e => e.Watches)
            .FirstOrDefaultAsync(e => e.TicketmasterEventId == ticketmasterEventId);

        if (ev is null) return null;

        return MapToDetail(ev);
    }

    private static EventDetailDto MapToDetail(Domain.Entities.Event ev)
    {
        var activeWatch = ev.Watches.FirstOrDefault(w => w.Status == Domain.Enums.WatchStatus.Active);

        return new EventDetailDto(
            ev.Id,
            ev.TicketmasterEventId,
            ev.Title,
            ev.Artist,
            ev.Venue,
            ev.City,
            ev.EventDate,
            ev.TicketmasterUrl,
            ev.ImageUrl,
            ev.Genre,
            activeWatch is not null,
            activeWatch?.Id,
            activeWatch?.Status.ToString()
        );
    }
}