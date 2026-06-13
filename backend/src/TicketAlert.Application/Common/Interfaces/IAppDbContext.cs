using Microsoft.EntityFrameworkCore;
using TicketAlert.Domain.Entities;

namespace TicketAlert.Application.Common.Interfaces;

public interface IAppDbContext
{
    DbSet<User> Users { get; }
    DbSet<Event> Events { get; }
    DbSet<Watch> Watches { get; }
    DbSet<Notification> Notifications { get; }
    DbSet<Payment> Payments { get; }
    DbSet<ApiPollingRecord> ApiPollingRecords { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
