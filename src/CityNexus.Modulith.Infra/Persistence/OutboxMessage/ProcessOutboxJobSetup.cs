using Microsoft.Extensions.Options;
using Quartz;

namespace CityNexus.Modulith.Infra.Persistence.OutboxMessage;

internal sealed class ProcessOutboxJobSetup : IConfigureOptions<QuartzOptions>
{
    public void Configure(QuartzOptions options)
    {
        const string jobName = nameof(ProcessOutboxJob);

        options
            .AddJob<ProcessOutboxJob>(configure => configure.WithIdentity(jobName))
            .AddTrigger(configure =>
                configure
                    .ForJob(jobName)
                    .WithSimpleSchedule(schedule =>
                        schedule.WithIntervalInSeconds(5).RepeatForever()
                    )
            );
    }
}
