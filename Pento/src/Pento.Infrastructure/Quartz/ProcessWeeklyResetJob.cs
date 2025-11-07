using Marten;
using Pento.Application.Abstractions.Clock;
using Pento.Domain.Shared;
using Pento.Domain.UserSubscriptions;
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
        IReadOnlyList<UserSubscriptionInstance> subscriptionInstances = await session.Query
            <UserSubscriptionInstance>()
            .Where(x => x.Status == SubscriptionStatus.Active)
            .ToListAsync(context.CancellationToken);
        foreach (UserSubscriptionInstance subscriptionInstance in subscriptionInstances)
        {
            foreach (Consumable consumable in subscriptionInstance.Consumables)
            {
                if (consumable.ResetInterval == Interval.Daily)
                {
                    consumable.ResetUsage();
                }
                if (consumable.ResetInterval == Interval.Weekly && now.Subtract(subscriptionInstance.StartDateUtc).Days % 7 == 0)
                {
                    consumable.ResetUsage();
                }
                if (consumable.ResetInterval == Interval.Monthly && now.Subtract(subscriptionInstance.StartDateUtc).Days % 30 == 0)
                {
                    consumable.ResetUsage();
                }
            }
            if (now.Day == 1)
            {

            }

            if (now.DayOfWeek == DayOfWeek.Monday)
            {

            }
        }
    }
}
