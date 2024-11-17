using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CityNexus.Modulith.Infra.Persistence.EF.EntitiesConfiguration;

public sealed class OutboxEntityTypeConfiguration : IEntityTypeConfiguration<Outbox>
{
    public void Configure(EntityTypeBuilder<Outbox> builder)
    {
        builder.HasKey(p => p.Id);
        builder.HasIndex(o => o.EventName);
        builder.Property(o => o.Payload).HasColumnType("jsonb");
    }
}
