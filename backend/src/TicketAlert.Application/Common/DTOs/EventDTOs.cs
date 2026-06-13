namespace TicketAlert.Application.Common.DTOs;

public record EventSearchRequest(
    string? Keyword,
    string? Artist,
    string? City,
    string? Genre,
    DateTime? StartDate,
    DateTime? EndDate,
    int Page = 1,
    int PageSize = 20
);

public record EventSearchResponse(
    IReadOnlyList<EventDto> Events,
    int TotalCount,
    int Page,
    int PageSize
);

public record EventDto(
    Guid Id,
    string TicketmasterEventId,
    string Title,
    string? Artist,
    string? Venue,
    string? City,
    DateTime EventDate,
    string TicketmasterUrl,
    string? ImageUrl,
    string? Genre
);

public record EventDetailDto(
    Guid Id,
    string TicketmasterEventId,
    string Title,
    string? Artist,
    string? Venue,
    string? City,
    DateTime EventDate,
    string TicketmasterUrl,
    string? ImageUrl,
    string? Genre,
    bool IsWatched,
    Guid? WatchId,
    string? WatchStatus
);
