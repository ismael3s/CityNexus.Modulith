using CityNexus.Modulith.Infra.Persistence.OutboxMessage;
using CityNexus.Modulith.People.Domain.Entities.People;
using CityNexus.Modulith.SharedKernel.Abstractions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CityNexus.Modulith.People.Infra.Persistence.EF;

public sealed class PersonDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Person> People { get; set; }

    public DbSet<Outbox> Outbox { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var options = optionsBuilder.UseSnakeCaseNamingConvention();
        base.OnConfiguring(options);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("people");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PersonDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        AddDomainEventsAsOutboxMessages();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void AddDomainEventsAsOutboxMessages()
    {
        var outboxMessages = ChangeTracker
            .Entries<Entity>()
            .Select(e => e.Entity)
            .SelectMany(entity =>
            {
                var domainEvents = entity.GetDomainEvents();
                entity.ClearDomainEvents();
                return domainEvents;
            })
            .Select(domainEvent => new Outbox(
                domainEvent.GetType().Name,
                JsonConvert.SerializeObject(
                    domainEvent,
                    new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All,
                        ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    }
                )
            ))
            .ToList();
        AddRange(outboxMessages);
    }
}
