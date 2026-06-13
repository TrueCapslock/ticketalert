using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using TicketAlert.Application.Common.Interfaces;
using TicketAlert.Domain.Entities;

namespace TicketAlert.Infrastructure.Services.Notifications;

public class EmailNotificationService : INotificationService
{
    private readonly SmtpClient _smtp;
    private readonly string _fromAddress;

    public EmailNotificationService(IConfiguration config)
    {
        _fromAddress = config["Email:FromAddress"] ?? "noreply@ticketalert.no";
        var host = config["Email:SmtpHost"];
        var port = int.TryParse(config["Email:SmtpPort"], out var p) ? p : 587;
        var username = config["Email:Username"] ?? "";
        var password = config["Email:Password"] ?? "";

        _smtp = new SmtpClient(host ?? "localhost")
        {
            Port = port,
            Credentials = new NetworkCredential(username, password),
            EnableSsl = !string.IsNullOrEmpty(host)
        };
    }

    public async Task SendTicketAlertAsync(User user, Watch watch, Event @event)
    {
        var subject = $"🎵 Billetter tilgjengelig: {@event.Title}!";
        var body = $@"
Hei {user.Name ?? user.Email},

Billetter til {@event.Title} er NÅ tilgjengelige!

Dato: {@event.EventDate:dd.MM.yyyy HH:mm}
Sted: {@event.Venue}, {@event.City}

Kjøp billetter her:
{@event.TicketmasterUrl}

Vennlig hilsen,
TicketAlert";

        await SendEmailAsync(user.Email, subject, body);
    }

    public async Task SendEmailVerificationAsync(User user, string token)
    {
        var subject = "Bekreft e-posten din - TicketAlert";
        var body = $@"
Hei {user.Name ?? user.Email},

Bekreft e-posten din ved å klikke på lenken under:

https://ticketalert.no/verify-email?token={token}

Vennlig hilsen,
TicketAlert";

        await SendEmailAsync(user.Email, subject, body);
    }

    public async Task SendPasswordResetEmailAsync(User user, string token)
    {
        var subject = "Tilbakestill passord - TicketAlert";
        var body = $@"
Hei {user.Name ?? user.Email},

Klikk på lenken under for å tilbakestille passordet ditt:

https://ticketalert.no/reset-password?token={token}

Lenken er gyldig i 1 time.

Vennlig hilsen,
TicketAlert";

        await SendEmailAsync(user.Email, subject, body);
    }

    private async Task SendEmailAsync(string to, string subject, string body)
    {
        var mail = new MailMessage(_fromAddress, to, subject, body)
        {
            IsBodyHtml = false
        };

        await _smtp.SendMailAsync(mail);
    }
}
