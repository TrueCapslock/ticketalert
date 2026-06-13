namespace TicketAlert.Application.Common.DTOs;

public record CreateWatchRequest(
    string TicketmasterEventId,
    string Title,
    string? Artist,
    string? Venue,
    string? City,
    DateTime EventDate,
    string TicketmasterUrl,
    string? ImageUrl
);

public record WatchDto(
    Guid Id,
    Guid EventId,
    string EventTitle,
    string? Artist,
    string? Venue,
    DateTime EventDate,
    string TicketmasterUrl,
    string Status,
    DateTime CreatedAt,
    DateTime ExpiresAt,
    DateTime? TriggeredAt
);
