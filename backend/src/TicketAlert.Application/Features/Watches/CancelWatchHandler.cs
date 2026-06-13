using Microsoft.EntityFrameworkCore;
using TicketAlert.Application.Common.Interfaces;
using TicketAlert.Domain.Enums;

namespace TicketAlert.Application.Features.Watches;

public class CancelWatchHandler : ICancelWatchHandler
{
    private readonly IAppDbContext _db;

    public CancelWatchHandler(IAppDbContext db) => _db = db;

    public async Task HandleAsync(Guid userId, Guid watchId)
    {
        var watch = await _db.Watches
            .FirstOrDefaultAsync(w => w.Id == watchId && w.UserId == userId);

        if (watch is null)
            throw new KeyNotFoundException("Watch not found.");

        if (watch.Status != WatchStatus.Active)
            throw new InvalidOperationException("Only active watches can be cancelled.");

        watch.Status = WatchStatus.Cancelled;
        watch.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }
}
