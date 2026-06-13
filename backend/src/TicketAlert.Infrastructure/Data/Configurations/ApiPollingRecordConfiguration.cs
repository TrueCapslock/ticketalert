using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketAlert.Domain.Entities;

namespace TicketAlert.Infrastructure.Data.Configurations;

public class ApiPollingRecordConfiguration : IEntityTypeConfiguration<ApiPollingRecord>
{
    public void Configure(EntityTypeBuilder<ApiPollingRecord> builder)
    {
        builder.ToTable("ApiPollingHistory");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.RawResponse)
            .HasColumnType("jsonb");

        builder.HasOne(r => r.Event)
            .WithMany(e => e.PollingRecords)
            .HasForeignKey(r => r.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(r => r.EventId);
        builder.HasIndex(r => r.PolledAt);
        builder.HasIndex(r => r.TicketsAvailable);
    }
}
