using TicketAlert.Application.Common.DTOs;

namespace TicketAlert.Application.Features.Notifications;

public interface IGetUserNotificationsHandler
{
    Task<NotificationListResponse> HandleAsync(Guid userId, int page, int pageSize);
}

public interface IMarkNotificationReadHandler
{
    Task HandleAsync(Guid userId, Guid notificationId);
}
