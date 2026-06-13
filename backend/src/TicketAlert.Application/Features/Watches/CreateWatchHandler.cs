using Microsoft.EntityFrameworkCore;
using TicketAlert.Application.Common.DTOs;
using TicketAlert.Application.Common.Interfaces;
using TicketAlert.Domain.Entities;
using TicketAlert.Domain.Enums;

namespace TicketAlert.Application.Features.Watches;

public class CreateWatchHandler : ICreateWatchHandler
{
    private readonly IAppDbContext _db;
    private readonly IPaymentService _payment;

    public CreateWatchHandler(IAppDbContext db, IPaymentService payment)
    {
        _db = db;
        _payment = payment;
    }

    public async Task<WatchDto> HandleAsync(Guid userId, CreateWatchRequest request)
    {
        var existingEvent = await _db.Events
            .FirstOrDefaultAsync(e => e.TicketmasterEventId == request.TicketmasterEventId);

        if (existingEvent is null)
        {
            existingEvent = new Event
            {
                Id = Guid.NewGuid(),
                TicketmasterEventId = request.TicketmasterEventId,
                Title = request.Title,
                Artist = request.Artist,
                Venue = request.Venue,
                City = request.City,
                EventDate = request.EventDate,
                TicketmasterUrl = request.TicketmasterUrl,
                ImageUrl = request.ImageUrl
            };
            _db.Events.Add(existingEvent);
        }

        var hasActiveWatch = await _db.Watches
            .AnyAsync(w => w.UserId == userId
                && w.EventId == existingEvent.Id
                && w.Status == WatchStatus.Active);

        if (hasActiveWatch)
            throw new InvalidOperationException("You already have an active watch for this event.");

        var watch = new Watch
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            EventId = existingEvent.Id,
            ExpiresAt = existingEvent.EventDate
        };

        _db.Watches.Add(watch);
        await _db.SaveChangesAsync();

        return MapToDto(watch, existingEvent);
    }

    private static WatchDto MapToDto(Watch watch, Event ev) => new(
        watch.Id, ev.Id, ev.Title, ev.Artist, ev.Venue,
        ev.EventDate, ev.TicketmasterUrl, watch.Status.ToString(),
        watch.CreatedAt, watch.ExpiresAt, watch.TriggeredAt);
}
