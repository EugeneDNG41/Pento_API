using Microsoft.Extensions.Options;
using Pento.Infrastructure.Outbox;
using Quartz;

namespace Pento.Infrastructure.Quartz;
#pragma warning disable S125 // Sections of code should not be commented out
internal sealed class QuartzJobsSetup(IOptions<OutboxOptions> outboxOptions) : IConfigureOptions<QuartzOptions>
{
    public void Configure(QuartzOptions options)
    {
        const string expirationTracker = nameof(ProcessExpirationDateTrackingJob);
        const string limitReset = nameof(ProcessLimitResetJob);
        const string jobName = nameof(ProcessOutboxMessagesJob);


        options
            .AddJob<ProcessExpirationDateTrackingJob>(configure => configure.WithIdentity(expirationTracker))
            .AddTrigger(configure =>
                configure
                    .ForJob(expirationTracker)
                    .WithCronSchedule("0 0 0/12 ? * * *")// Every 12 hours 
                    .Build());
        options
            .AddJob<ProcessLimitResetJob>(configure => configure.WithIdentity(limitReset))
            .AddTrigger(configure =>
                configure
                    .ForJob(limitReset)
                    .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(0, 0)) // Every day at midnight
                    .Build());
        options
            .AddJob<ProcessOutboxMessagesJob>(configure => configure.WithIdentity(jobName))
            .AddTrigger(configure =>
                configure
                    .ForJob(jobName)
                    .WithSimpleSchedule(schedule =>
                        schedule.WithIntervalInSeconds(outboxOptions.Value.IntervalInSeconds).RepeatForever()));
    }
}
