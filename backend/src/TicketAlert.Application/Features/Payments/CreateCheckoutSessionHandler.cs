using Microsoft.EntityFrameworkCore;
using TicketAlert.Application.Common.DTOs;
using TicketAlert.Application.Common.Interfaces;
using TicketAlert.Domain.Entities;
using TicketAlert.Domain.Enums;

namespace TicketAlert.Application.Features.Payments;

public class CreateCheckoutSessionHandler : ICreateCheckoutSessionHandler
{
    private readonly IAppDbContext _db;
    private readonly IPaymentService _payment;

    public CreateCheckoutSessionHandler(IAppDbContext db, IPaymentService payment)
    {
        _db = db;
        _payment = payment;
    }

    public async Task<CheckoutResponse> HandleAsync(Guid userId, CreateCheckoutRequest request)
    {
        var ev = await _db.Events.FindAsync(request.EventId)
            ?? throw new KeyNotFoundException("Event not found.");

        var existingActive = await _db.Watches
            .AnyAsync(w => w.UserId == userId && w.EventId == request.EventId && w.Status == WatchStatus.Active);

        if (existingActive)
            throw new InvalidOperationException("Active watch already exists for this event.");

        var watch = new Watch
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            EventId = request.EventId,
            ExpiresAt = ev.EventDate
        };

        var price = await _payment.GetPriceInNokAsync();
        var sessionUrl = await _payment.CreateCheckoutSessionAsync(
            userId, watch.Id, price, "nok",
            request.SuccessUrl, request.CancelUrl);

        _db.Watches.Add(watch);
        await _db.SaveChangesAsync();

        return new CheckoutResponse(sessionUrl, string.Empty);
    }
}
