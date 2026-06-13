namespace TicketAlert.Domain.Entities;

public class ApiPollingRecord
{
    public Guid Id { get; set; }
    public Guid EventId { get; set; }
    public bool TicketsAvailable { get; set; }
    public int? TotalCount { get; set; }
    public string? RawResponse { get; set; }
    public int? HttpStatusCode { get; set; }
    public long DurationMs { get; set; }
    public DateTime PolledAt { get; set; } = DateTime.UtcNow;

    public Event Event { get; set; } = null!;
}
