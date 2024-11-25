using CityNexus.Modulith.People.Domain.Entities.People.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace CityNexus.Modulith.Infra;

public sealed class OrderConsumer : IConsumer<RegisteredPersonDomainEvent>
{
    private readonly ILogger<OrderConsumer> _logger;

    public OrderConsumer(ILogger<OrderConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<RegisteredPersonDomainEvent> context)
    {
        Console.WriteLine(context.Message.Id);
        _logger.LogInformation(context.Message.Id.ToString());
    }
}
