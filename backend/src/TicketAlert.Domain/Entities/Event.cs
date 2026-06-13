namespace TicketAlert.Domain.Entities;

public class Event
{
    public Guid Id { get; set; }
    public string TicketmasterEventId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Artist { get; set; }
    public string? Venue { get; set; }
    public string? City { get; set; }
    public DateTime EventDate { get; set; }
    public string TicketmasterUrl { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string? Genre { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<Watch> Watches { get; set; } = new List<Watch>();
    public ICollection<ApiPollingRecord> PollingRecords { get; set; } = new List<ApiPollingRecord>();
}
