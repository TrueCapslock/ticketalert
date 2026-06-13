using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TicketAlert.Application.Common.Interfaces;
using TicketAlert.Domain.Entities;
using TicketAlert.Domain.Enums;

namespace TicketAlert.Infrastructure.BackgroundJobs;

public class TicketPollingService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<TicketPollingService> _logger;

    public TicketPollingService(IServiceProvider services, ILogger<TicketPollingService> logger)
    {
        _services = services;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("TicketPollingService started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
                var tm = scope.ServiceProvider.GetRequiredService<ITicketmasterService>();
                var notification = scope.ServiceProvider.GetRequiredService<INotificationService>();

                var now = DateTime.UtcNow;
                var activeWatches = await db.Watches
                    .Include(w => w.Event)
                    .Include(w => w.User)
                    .Where(w => w.Status == WatchStatus.Active && w.ExpiresAt > now)
                    .ToListAsync(stoppingToken);

                foreach (var watch in activeWatches)
                {
                    var interval = GetPollingInterval(watch.Event.EventDate, now);

                    var lastPoll = await db.ApiPollingRecords
                        .Where(r => r.EventId == watch.EventId)
                        .OrderByDescending(r => r.PolledAt)
                        .FirstOrDefaultAsync(stoppingToken);

                    if (lastPoll is not null && (now - lastPoll.PolledAt).TotalMinutes < interval)
                        continue;

                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    var (available, totalCount) = await tm.GetInventoryStatusAsync(
                        watch.Event.TicketmasterEventId,
                        watch.Event.TicketmasterUrl);
                    sw.Stop();

                    var record = new ApiPollingRecord
                    {
                        Id = Guid.NewGuid(),
                        EventId = watch.EventId,
                        TicketsAvailable = available,
                        TotalCount = totalCount,
                        HttpStatusCode = available ? 200 : 204,
                        DurationMs = sw.ElapsedMilliseconds,
                        PolledAt = now
                    };
                    db.ApiPollingRecords.Add(record);

                    if (available)
                    {
                        watch.Status = WatchStatus.Triggered;
                        watch.TriggeredAt = now;

                        var notif = new Notification
                        {
                            Id = Guid.NewGuid(),
                            WatchId = watch.Id,
                            UserId = watch.UserId,
                            Type = NotificationType.Email,
                            Recipient = watch.User.Email,
                            Subject = $"Billetter tilgjengelig: {watch.Event.Title}",
                            Body = $"Billetter til {watch.Event.Title} er tilgjengelige! {watch.Event.TicketmasterUrl}",
                            Sent = false
                        };
                        db.Notifications.Add(notif);
                        await db.SaveChangesAsync(stoppingToken);

                        try
                        {
                            await notification.SendTicketAlertAsync(watch.User, watch, watch.Event);
                            notif.Sent = true;
                            notif.SentAt = DateTime.UtcNow;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to send notification for watch {WatchId}", watch.Id);
                        }
                    }

                    await db.SaveChangesAsync(stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in TicketPollingService cycle");
            }

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }

        _logger.LogInformation("TicketPollingService stopped");
    }

    private static double GetPollingInterval(DateTime eventDate, DateTime now)
    {
        var daysUntilEvent = (eventDate - now).TotalDays;

        return daysUntilEvent switch
        {
            < 1 => 5,       // Same day: every 5 min
            < 7 => 15,      // Less than a week: every 15 min
            < 30 => 60,     // Less than a month: every hour
            _ => 360        // More than 30 days: every 6 hours
        };
    }
}
