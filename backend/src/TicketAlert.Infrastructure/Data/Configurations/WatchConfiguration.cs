using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketAlert.Domain.Entities;

namespace TicketAlert.Infrastructure.Data.Configurations;

public class WatchConfiguration : IEntityTypeConfiguration<Watch>
{
    public void Configure(EntityTypeBuilder<Watch> builder)
    {
        builder.ToTable("Watches");

        builder.HasKey(w => w.Id);

        builder.Property(w => w.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.HasOne(w => w.User)
            .WithMany(u => u.Watches)
            .HasForeignKey(w => w.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(w => w.Event)
            .WithMany(e => e.Watches)
            .HasForeignKey(w => w.EventId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(w => w.UserId);
        builder.HasIndex(w => w.EventId);
        builder.HasIndex(w => w.Status);
        builder.HasIndex(w => w.ExpiresAt);
        builder.HasIndex(w => new { w.Status, w.ExpiresAt });
    }
}
