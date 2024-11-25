using CityNexus.Modulith.People.Domain.Entities.People;
using CityNexus.Modulith.SharedKernel.VO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CityNexus.Modulith.People.Infra.Persistence.EF.EntityTypeConfiguration;

public sealed class PersonEntityTypeConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.HasIndex(p => p.Document).IsUnique();
        builder.HasIndex(p => p.Email).IsUnique();
        builder.Property(p => p.Name).HasConversion(p => p.Value, p => new Name(p));
        builder.Property(p => p.Email).HasConversion(p => p.Value, p => new Email(p));
        builder.Property(p => p.Document).HasConversion(p => p.Value, p => new Document(p));
        builder.HasKey(p => p.Id);
    }
}
