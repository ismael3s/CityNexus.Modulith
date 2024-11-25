using CityNexus.Modulith.Infra.Persistence.OutboxMessage;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CityNexus.Modulith.People.Infra.Persistence.EF.EntityTypeConfiguration;

public sealed class OutboxEntityTypeConfiguration : IEntityTypeConfiguration<Outbox>
{
    public void Configure(EntityTypeBuilder<Outbox> builder)
    {
        builder.HasKey(p => p.Id);
        builder.HasIndex(o => o.EventName);
        builder.Property(o => o.Payload).HasColumnType("jsonb");
    }
}
