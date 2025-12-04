using Microsoft.Extensions.Options;
using Pento.Infrastructure.Outbox;
using Quartz;

namespace Pento.Infrastructure.ThirdPartyServices.Quartz;
internal sealed class QuartzJobsSetup(IOptions<OutboxOptions> outboxOptions) : IConfigureOptions<QuartzOptions>
{
    public void Configure(QuartzOptions options)
    {
        const string expirationTracker = nameof(ProcessExpirationDateTrackingJob);
        const string limitReset = nameof(ProcessLimitResetJob);
        const string outbox = nameof(ProcessOutboxMessagesJob);
        const string paymentStatusTracker = nameof(ProcessPaymentStatusTrackingJob);


        options
            .AddJob<ProcessExpirationDateTrackingJob>(configure => configure.WithIdentity(expirationTracker))
            .AddTrigger(configure =>
                configure
                    .ForJob(expirationTracker)
                    .WithSimpleSchedule(schedule =>
                        schedule.WithIntervalInMinutes(30).RepeatForever())
                    .Build());
        options
            .AddJob<ProcessLimitResetJob>(configure => configure.WithIdentity(limitReset))
            .AddTrigger(configure =>
                configure
                    .ForJob(limitReset)
                    .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(0, 0)) // Every day at midnight
                    .Build());
        options
            .AddJob<ProcessOutboxMessagesJob>(configure => configure.WithIdentity(outbox))
            .AddTrigger(configure =>
                configure
                    .ForJob(outbox)
                    .WithSimpleSchedule(schedule =>
                        schedule.WithIntervalInSeconds(outboxOptions.Value.IntervalInSeconds).RepeatForever()));
        options.AddJob<ProcessPaymentStatusTrackingJob>(configure => configure.WithIdentity(paymentStatusTracker))
            .AddTrigger(configure =>
                configure
                    .ForJob(paymentStatusTracker)
                    .WithSimpleSchedule(schedule =>
                        schedule.WithIntervalInSeconds(60).RepeatForever()));
    }
}
