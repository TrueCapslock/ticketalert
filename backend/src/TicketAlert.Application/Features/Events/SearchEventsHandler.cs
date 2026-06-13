using Microsoft.EntityFrameworkCore;
using TicketAlert.Application.Common.DTOs;
using TicketAlert.Application.Common.Interfaces;

namespace TicketAlert.Application.Features.Events;

public class SearchEventsHandler : ISearchEventsHandler
{
    private readonly IAppDbContext _db;
    private readonly ITicketmasterService _tm;

    public SearchEventsHandler(IAppDbContext db, ITicketmasterService tm)
    {
        _db = db;
        _tm = tm;
    }

    public async Task<EventSearchResponse> HandleAsync(EventSearchRequest request)
    {
        var localEvents = _db.Events.AsQueryable();

        if (!string.IsNullOrEmpty(request.Keyword))
            localEvents = localEvents.Where(e =>
                e.Title.Contains(request.Keyword) ||
                (e.Artist != null && e.Artist.Contains(request.Keyword)));

        if (!string.IsNullOrEmpty(request.Artist))
            localEvents = localEvents.Where(e => e.Artist != null && e.Artist.Contains(request.Artist));

        if (!string.IsNullOrEmpty(request.City))
            localEvents = localEvents.Where(e => e.City != null && e.City.Contains(request.City));

        if (!string.IsNullOrEmpty(request.Genre))
            localEvents = localEvents.Where(e => e.Genre == request.Genre);

        if (request.StartDate.HasValue)
            localEvents = localEvents.Where(e => e.EventDate >= request.StartDate.Value);

        if (request.EndDate.HasValue)
            localEvents = localEvents.Where(e => e.EventDate <= request.EndDate.Value);

        var totalCount = await localEvents.CountAsync();
        var events = await localEvents
            .OrderBy(e => e.EventDate)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(e => new EventDto(
                e.Id, e.TicketmasterEventId, e.Title, e.Artist,
                e.Venue, e.City, e.EventDate, e.TicketmasterUrl,
                e.ImageUrl, e.Genre))
            .ToListAsync();

        if (!events.Any() && !string.IsNullOrEmpty(request.Keyword))
        {
            var tmEvents = await _tm.SearchEventsAsync(
                request.Keyword, request.Artist, request.City,
                request.Page, request.PageSize);

            foreach (var ev in tmEvents)
            {
                var existing = await _db.Events
                    .FirstOrDefaultAsync(e => e.TicketmasterEventId == ev.TicketmasterEventId);
                if (existing is null)
                {
                    _db.Events.Add(ev);
                }
            }
            await _db.SaveChangesAsync();

            events = tmEvents.Select(e => new EventDto(
                e.Id, e.TicketmasterEventId, e.Title, e.Artist,
                e.Venue, e.City, e.EventDate, e.TicketmasterUrl,
                e.ImageUrl, e.Genre)).ToList();
            totalCount = events.Count;
        }

        return new EventSearchResponse(events, totalCount, request.Page, request.PageSize);
    }
}
