using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketAlert.Domain.Entities;

namespace TicketAlert.Infrastructure.Data.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.StripeSessionId)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(p => p.StripePaymentIntentId)
            .HasMaxLength(256);

        builder.Property(p => p.Amount)
            .HasPrecision(10, 2);

        builder.Property(p => p.Currency)
            .IsRequired()
            .HasMaxLength(4);

        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.HasOne(p => p.User)
            .WithMany(u => u.Payments)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Watch)
            .WithOne(w => w.Payment)
            .HasForeignKey<Payment>(p => p.WatchId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => p.UserId);
        builder.HasIndex(p => p.WatchId)
            .IsUnique();
        builder.HasIndex(p => p.StripeSessionId)
            .IsUnique();
        builder.HasIndex(p => p.StripePaymentIntentId);
        builder.HasIndex(p => p.Status);
    }
}
