using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketAlert.Domain.Entities;

namespace TicketAlert.Infrastructure.Data.Configurations;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.ToTable("Events");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.TicketmasterEventId)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(e => e.Artist)
            .HasMaxLength(256);

        builder.Property(e => e.Venue)
            .HasMaxLength(256);

        builder.Property(e => e.City)
            .HasMaxLength(128);

        builder.Property(e => e.TicketmasterUrl)
            .IsRequired()
            .HasMaxLength(2048);

        builder.Property(e => e.ImageUrl)
            .HasMaxLength(2048);

        builder.Property(e => e.Genre)
            .HasMaxLength(128);

        builder.HasIndex(e => e.TicketmasterEventId)
            .IsUnique();

        builder.HasIndex(e => e.EventDate);

        builder.HasIndex(e => new { e.Artist, e.EventDate });

        builder.HasIndex(e => e.Title);
    }
}
