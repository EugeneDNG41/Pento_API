using Quartz;
using Quartz.Impl.Matchers;

namespace Pento.Infrastructure.External.Quartz;

public sealed class JobScheduler(ISchedulerFactory schedulerFactory)
{
    private async Task<IScheduler> GetSchedulerAsync() => await schedulerFactory.GetScheduler();

    private static JobKey JKey(string key, string? group) => new(key, group ?? "default");
    private static TriggerKey TKey(string key, string? group) => new(key, group ?? "default");

    private static IJobDetail BuildJob<TJob>(string key, string? group, IDictionary<string, object>? data)
        where TJob : IJob
    {
        JobBuilder jb = JobBuilder
            .Create<TJob>()
            .WithIdentity(JKey(key, group))
            .StoreDurably(false)
            .RequestRecovery(true);

        if (data is { Count: > 0 })
        {
            jb.UsingJobData(new JobDataMap(data));
        }

        return jb.Build();
    }
    public async Task<bool> ScheduleOnce<TJob>(
        string key,
        DateTimeOffset runAtUtc,
        IDictionary<string, object>? data = null,
        string? group = null,
        bool replaceIfExists = true,
        CancellationToken ct = default) where TJob : IJob
    {
        IScheduler sched = await GetSchedulerAsync();
        IJobDetail job = BuildJob<TJob>(key, group, data);
        ITrigger trigger = TriggerBuilder.Create()
            .WithIdentity(TKey(key, group))
            .StartAt(runAtUtc)
            .WithSimpleSchedule(s => s.WithRepeatCount(0))
            .Build();

        if (replaceIfExists)
        {
            await DeleteIfExists(sched, key, group, ct);
        }

        await sched.ScheduleJob(job, trigger, ct);
        return true;
    }

    public async Task<bool> ScheduleCron<TJob>(
        string key,
        string cronExpression,
        IDictionary<string, object>? data = null,
        string? group = null,
        TimeZoneInfo? timeZone = null,
        bool replaceIfExists = true,
        CancellationToken ct = default) where TJob : IJob
    {
        IScheduler sched = await GetSchedulerAsync();

        IJobDetail job = BuildJob<TJob>(key, group, data);

        TriggerBuilder tb = TriggerBuilder.Create()
            .WithIdentity(TKey(key, group))
            .WithSchedule(CronScheduleBuilder
                .CronSchedule(cronExpression)
                .InTimeZone(timeZone ?? TimeZoneInfo.Utc)
                .WithMisfireHandlingInstructionDoNothing());

        ITrigger trigger = tb.Build();

        if (replaceIfExists)
        {
            await DeleteIfExists(sched, key, group, ct);
        }

        await sched.ScheduleJob(job, trigger, ct);
        return true;
    }

    public async Task<bool> ScheduleSimple<TJob>(
        string key,
        TimeSpan interval,
        int? repeatCount = null,
        DateTimeOffset? startAtUtc = null,
        IDictionary<string, object>? data = null,
        string? group = null,
        bool replaceIfExists = true,
        CancellationToken ct = default) where TJob : IJob
    {
        IScheduler sched = await GetSchedulerAsync();
        IJobDetail job = BuildJob<TJob>(key, group, data);

        ITrigger trigger = TriggerBuilder.Create()
            .WithIdentity(TKey(key, group))
            .StartAt(startAtUtc ?? DateTimeOffset.UtcNow)
            .WithSimpleSchedule(x => x.WithInterval(interval).WithRepeatCount(repeatCount ?? 0)
            .WithInterval(interval))
            .Build();

        if (replaceIfExists)
        {
            await DeleteIfExists(sched, key, group, ct);
        }

        await sched.ScheduleJob(job, trigger, ct);
        return true;
    }

    public async Task<bool> Cancel(string key, string? group = null, CancellationToken ct = default)
    {
        IScheduler sched = await GetSchedulerAsync();
        JobKey jobKey = JKey(key, group);
        bool result = await sched.DeleteJob(jobKey, ct);
        return result;
    }

    public async Task<bool> Pause(string key, string? group = null, CancellationToken ct = default)
    {
        IScheduler sched = await GetSchedulerAsync();
        await sched.PauseJob(JKey(key, group), ct);
        return true;
    }

    public async Task<bool> Resume(string key, string? group = null, CancellationToken ct = default)
    {
        IScheduler sched = await GetSchedulerAsync();
        await sched.ResumeJob(JKey(key, group), ct);
        return true;
    }

    public async Task<bool> Exists(string key, string? group = null, CancellationToken ct = default)
    {
        IScheduler sched = await GetSchedulerAsync();
        return await sched.CheckExists(JKey(key, group), ct);
    }

    public async Task<IReadOnlyList<(JobKey job, TriggerKey trigger)>> List(string? group = null, CancellationToken ct = default)
    {
        IScheduler sched = await GetSchedulerAsync();
        GroupMatcher<JobKey> matcher = group is null ? GroupMatcher<JobKey>.AnyGroup() : GroupMatcher<JobKey>.GroupEquals(group);
        IReadOnlyCollection<JobKey> jobs = await sched.GetJobKeys(matcher, ct);
        var result = new List<(JobKey, TriggerKey)>();
        foreach (JobKey jk in jobs)
        {
            IReadOnlyCollection<ITrigger> triggers = await sched.GetTriggersOfJob(jk, ct);
            foreach (ITrigger t in triggers)
            {
                result.Add((jk, t.Key));
            }
        }
        return result;
    }

    private static async Task DeleteIfExists(IScheduler sched, string key, string? group, CancellationToken ct)
    {
        var jk = new JobKey(key, group ?? "default");
        if (await sched.CheckExists(jk, ct))
        {
            // Also deletes triggers attached to the job
            await sched.DeleteJob(jk, ct);
        }
    }
}
