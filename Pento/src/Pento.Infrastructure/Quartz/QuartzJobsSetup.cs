using Microsoft.Extensions.Options;
using Quartz;

namespace Pento.Infrastructure.Quartz;

internal sealed class QuartzJobsSetup : IConfigureOptions<QuartzOptions>
{

    public void Configure(QuartzOptions options)
    {
        const string expirationTracker = nameof(ProcessExpirationDateTrackingJob);
        const string dailyReset = nameof(ProcessDailyResetJob);
        const string weeklyReset = nameof(ProcessWeeklyResetJob);
        const string monthlyReset = nameof(ProcessMonthlyResetJob);

        options
            .AddJob<ProcessExpirationDateTrackingJob>(configure => configure.WithIdentity(expirationTracker))
            .AddTrigger(configure =>
                configure
                    .ForJob(expirationTracker)
                    .WithCronSchedule("0 0 0/12 ? * * *")// Every 12 hours 
                    .Build());
        options
            .AddJob<ProcessDailyResetJob>(configure => configure.WithIdentity(dailyReset))
            .AddTrigger(configure =>
                configure
                    .ForJob(dailyReset)
                    .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(0, 0)) // Every day at midnight
                    .Build());
        options
            .AddJob<ProcessWeeklyResetJob>(configure => configure.WithIdentity(weeklyReset))
            .AddTrigger(configure =>
                configure
                    .ForJob(weeklyReset)
                    .WithSchedule(CronScheduleBuilder.WeeklyOnDayAndHourAndMinute(DayOfWeek.Monday, 0, 0)) // Every week on Monday at midnight
                    .Build());
        options
            .AddJob<ProcessMonthlyResetJob>(configure => configure.WithIdentity(monthlyReset))
            .AddTrigger(configure =>
                configure
                    .ForJob(monthlyReset)
                    .WithSchedule(CronScheduleBuilder.MonthlyOnDayAndHourAndMinute(1, 0, 0)) // Every month on the 1st at midnight
                    .Build());
    }
}
