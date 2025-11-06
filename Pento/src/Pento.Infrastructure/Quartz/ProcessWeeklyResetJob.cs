using Marten;
using Pento.Application.Abstractions.Clock;
using Quartz;

namespace Pento.Infrastructure.Quartz;
#pragma warning disable CS9113 // Parameter is unread.
#pragma warning disable CS1998 
#pragma warning disable S108 // Nested blocks of code should not be left empty
[DisallowConcurrentExecution]
internal sealed class ProcessLimitResetJob(
    IDateTimeProvider dateTimeProvider,

    IDocumentSession session) : IJob

{
    public async Task Execute(IJobExecutionContext context)
    {
        DateTimeOffset now = dateTimeProvider.UtcNowOffset;

        if (now.Day == 1)
        {

        }

        if (now.DayOfWeek == DayOfWeek.Monday)
        {

        }

    }
}
