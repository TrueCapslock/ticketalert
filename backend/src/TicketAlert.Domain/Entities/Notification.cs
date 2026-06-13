using TicketAlert.Domain.Enums;

namespace TicketAlert.Domain.Entities;

public class Notification
{
    public Guid Id { get; set; }
    public Guid WatchId { get; set; }
    public Guid UserId { get; set; }
    public NotificationType Type { get; set; }
    public string Recipient { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool Sent { get; set; }
    public DateTime? SentAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Watch Watch { get; set; } = null!;
    public User User { get; set; } = null!;
}
