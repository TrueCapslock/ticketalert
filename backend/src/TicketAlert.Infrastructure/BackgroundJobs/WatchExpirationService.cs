using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TicketAlert.Application.Common.Interfaces;
using TicketAlert.Domain.Enums;

namespace TicketAlert.Infrastructure.BackgroundJobs;

public class WatchExpirationService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<WatchExpirationService> _logger;

    public WatchExpirationService(IServiceProvider services, ILogger<WatchExpirationService> logger)
    {
        _services = services;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("WatchExpirationService started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<IAppDbContext>();

                var now = DateTime.UtcNow;
                var expired = await db.Watches
                    .Where(w => w.Status == WatchStatus.Active && w.ExpiresAt <= now)
                    .ToListAsync(stoppingToken);

                foreach (var watch in expired)
                {
                    watch.Status = WatchStatus.Expired;
                    watch.UpdatedAt = now;
                }

                if (expired.Count > 0)
                {
                    await db.SaveChangesAsync(stoppingToken);
                    _logger.LogInformation("Expired {Count} watches", expired.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in WatchExpirationService");
            }

            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}
