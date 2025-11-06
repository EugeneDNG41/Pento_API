using Marten;
using Pento.Application.Abstractions.Clock;
using Quartz;

namespace Pento.Infrastructure.Quartz;
#pragma warning disable CS9113 // Parameter is unread.
#pragma warning disable S108 // Nested blocks of code should not be left empty
[DisallowConcurrentExecution]
internal sealed class ProcessDailyResetJob(
    IDateTimeProvider dateTimeProvider,

    IDocumentSession session) : IJob

{
    public async Task Execute(IJobExecutionContext context)
    {
        DateTime now = dateTimeProvider.UtcNow;
        if (now.DayOfWeek == DayOfWeek.Monday)

        {

        }

    }
}
[DisallowConcurrentExecution]
internal sealed class ProcessWeeklyResetJob : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
    }
}
[DisallowConcurrentExecution]
internal sealed class ProcessMonthlyResetJob : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {

    }
}
