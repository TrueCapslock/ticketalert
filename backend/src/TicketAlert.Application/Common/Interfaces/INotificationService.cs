using TicketAlert.Domain.Entities;

namespace TicketAlert.Application.Common.Interfaces;

public interface INotificationService
{
    Task SendTicketAlertAsync(User user, Watch watch, Event @event);
    Task SendEmailVerificationAsync(User user, string token);
    Task SendPasswordResetEmailAsync(User user, string token);
}
