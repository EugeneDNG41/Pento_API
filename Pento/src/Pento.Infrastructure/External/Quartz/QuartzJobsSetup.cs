using Microsoft.Extensions.Options;
using Pento.Infrastructure.Utility.Outbox;
using Quartz;

namespace Pento.Infrastructure.External.Quartz;
internal sealed class QuartzJobsSetup(IOptions<OutboxOptions> outboxOptions) : IConfigureOptions<QuartzOptions>
{
    public void Configure(QuartzOptions options)
    {
        const string expirationTracker = nameof(ProcessExpirationDateTrackingJob);
        const string entitlementReset = nameof(ProcessEntitlementResetJob);
        const string outbox = nameof(ProcessOutboxMessagesJob);
        const string paymentStatusTracker = nameof(ProcessPaymentStatusTrackingJob);
        const string subscriptionTracker = nameof(ProcessSubscriptionTrackingJob);


        options
            .AddJob<ProcessExpirationDateTrackingJob>(configure => configure.WithIdentity(expirationTracker))
            .AddTrigger(configure =>
                configure
                    .ForJob(expirationTracker)
                    .WithSimpleSchedule(schedule =>
                        schedule.WithIntervalInMinutes(30).RepeatForever())
                    .Build());
        options
            .AddJob<ProcessEntitlementResetJob>(configure => configure.WithIdentity(entitlementReset))
            .AddTrigger(configure =>
                configure
                    .ForJob(entitlementReset)
                    .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(0, 0)) // Every day at midnight
                    .Build());
        options
            .AddJob<ProcessSubscriptionTrackingJob>(configure => configure.WithIdentity(subscriptionTracker))
            .AddTrigger(configure =>
                configure
                    .ForJob(subscriptionTracker)
                    .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(6, 0)) // Every day at 4AM
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
