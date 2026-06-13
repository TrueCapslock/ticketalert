namespace TicketAlert.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? Name { get; set; }
    public bool EmailVerified { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<Watch> Watches { get; set; } = new List<Watch>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
