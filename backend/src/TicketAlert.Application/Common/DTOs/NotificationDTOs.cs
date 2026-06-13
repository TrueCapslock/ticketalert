namespace TicketAlert.Application.Common.DTOs;

public record NotificationDto(
    Guid Id,
    Guid WatchId,
    string Type,
    string Subject,
    string? Body,
    bool Sent,
    DateTime? SentAt,
    DateTime CreatedAt
);

public record NotificationListResponse(
    IReadOnlyList<NotificationDto> Notifications,
    int TotalCount,
    int UnreadCount
);
