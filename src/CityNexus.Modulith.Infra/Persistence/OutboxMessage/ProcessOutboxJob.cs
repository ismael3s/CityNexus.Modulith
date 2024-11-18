using System.Data;
using CityNexus.Modulith.Application.Modules.Shared.Abstractions;
using CityNexus.Modulith.Domain.Modules.Shared.Abstractions;
using Dapper;
using MassTransit;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Quartz;

namespace CityNexus.Modulith.Infra.Persistence.OutboxMessage;

[DisallowConcurrentExecution]
public sealed class ProcessOutboxJob(
    ISqlConnectionFactory connectionFactory,
    ILogger<ProcessOutboxJob> logger,
    IBus publisher
) : IJob
{
    private static readonly JsonSerializerSettings JsonSerializerSettings =
        new()
        {
            TypeNameHandling = TypeNameHandling.All,
            MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead,
        };

    public async Task Execute(IJobExecutionContext context)
    {
        using var connection = connectionFactory.CreateConnection();
        using var transaction = connection.BeginTransaction();
        var messages = await GetOutboxMessages(connection, transaction);
        foreach (var message in messages)
        {
            Exception? exception = null;
            try
            {
                IDomainEvent domainEvent = JsonConvert.DeserializeObject<IDomainEvent>(
                    message.Payload,
                    JsonSerializerSettings
                )!;
                logger.LogInformation(
                    "Processing message {Id} {Type} ",
                    message.Id,
                    domainEvent.GetType().Name
                );
                await publisher.Publish((object)domainEvent, context.CancellationToken);
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            await connection.ExecuteAsync(
                "UPDATE outbox SET processed_at = @ProcessedAt, error = @Error WHERE id = @Id",
                new
                {
                    Id = message.Id,
                    ProcessedAt = DateTime.UtcNow,
                    Error = exception?.ToString(),
                }
            );
        }
        transaction.Commit();
    }

    private async Task<List<OutboxMessage>> GetOutboxMessages(
        IDbConnection connection,
        IDbTransaction transaction
    )
    {
        var query = """
                SELECT id, event_name, payload
                from outbox
                WHERE processed_at is null 
                LIMIT  20
                FOR UPDATE
            """;
        return (
            await connection.QueryAsync<OutboxMessage>(query, transaction: transaction)
        ).ToList();
    }

    internal sealed record OutboxMessage(Guid Id, string EventName, string Payload);
}
