using TicketAlert.Domain.Enums;

namespace TicketAlert.Domain.Entities;

public class Watch
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid EventId { get; set; }
    public WatchStatus Status { get; set; } = WatchStatus.Active;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }
    public DateTime? TriggeredAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public User User { get; set; } = null!;
    public Event Event { get; set; } = null!;
    public Payment? Payment { get; set; }
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
