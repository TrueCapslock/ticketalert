using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketAlert.Domain.Entities;

namespace TicketAlert.Infrastructure.Data.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications");

        builder.HasKey(n => n.Id);

        builder.Property(n => n.Recipient)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(n => n.Subject)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(n => n.Body)
            .IsRequired();

        builder.HasOne(n => n.Watch)
            .WithMany(w => w.Notifications)
            .HasForeignKey(n => n.WatchId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(n => n.User)
            .WithMany(u => u.Notifications)
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(n => n.UserId);
        builder.HasIndex(n => n.WatchId);
        builder.HasIndex(n => n.Sent);
    }
}
