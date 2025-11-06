using Microsoft.Extensions.Options;
using Quartz;

namespace Pento.Infrastructure.Quartz;

internal sealed class QuartzJobsSetup : IConfigureOptions<QuartzOptions>
{
    public void Configure(QuartzOptions options)
    {
        const string expirationTracker = nameof(ProcessExpirationDateTrackingJob);
        const string limitReset = nameof(ProcessLimitResetJob);


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
    }
}
