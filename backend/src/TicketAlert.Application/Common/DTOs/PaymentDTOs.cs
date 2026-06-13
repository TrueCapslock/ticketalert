namespace TicketAlert.Application.Common.DTOs;

public record CreateCheckoutRequest(
    Guid EventId,
    string SuccessUrl,
    string CancelUrl
);

public record CheckoutResponse(
    string SessionUrl,
    string SessionId
);

public record PaymentDto(
    Guid Id,
    Guid WatchId,
    string EventTitle,
    decimal Amount,
    string Currency,
    string Status,
    DateTime CreatedAt,
    DateTime? CompletedAt
);
