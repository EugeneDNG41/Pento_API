using Dapper;
using Marten;
using Pento.Application.Abstractions.Clock;
using Pento.Application.Abstractions.Data;
using Pento.Domain.PointBalances.Events;
using Pento.Domain.PointBalances.Projections;
using Pento.Domain.PointCaps;
using Pento.Domain.Shared;
using Pento.Domain.UserSubscriptions;
using Quartz;

namespace Pento.Infrastructure.Quartz;
[DisallowConcurrentExecution]
internal sealed class ProcessLimitResetJob(
    IDateTimeProvider dateTimeProvider,
    IGenericRepository<PointCap> pointCapRepository,
    IDocumentSession session) : IJob

{
    public async Task Execute(IJobExecutionContext context)
    {
        DateTimeOffset now = dateTimeProvider.UtcNowOffset;
        await ExecuteConsumableReset(now, context.CancellationToken);
        await ExecutePointCapReset(now, context.CancellationToken);
    }

    private async Task ExecuteConsumableReset(DateTimeOffset now, CancellationToken cancellationToken)
    {
        IReadOnlyList<UserSubscriptionInstance> subscriptionInstances = await session.Query
            <UserSubscriptionInstance>()
            .Where(x => x.Status == SubscriptionStatus.Active)
            .ToListAsync(cancellationToken);
        foreach (UserSubscriptionInstance subscriptionInstance in subscriptionInstances)
        {
            foreach (Guid featureId in subscriptionInstance.Consumables.Keys)
            {
                if (subscriptionInstance.Consumables[featureId].ResetInterval == Interval.Daily)
                {
                    session.Events.Append(subscriptionInstance.Id, new ConsumableReset(featureId));
                }
                if (subscriptionInstance.Consumables[featureId].ResetInterval == Interval.Weekly && now.Subtract(subscriptionInstance.StartDateUtc).Days % 7 == 0)
                {
                    session.Events.Append(subscriptionInstance.Id, new ConsumableReset(featureId));
                }
                if (subscriptionInstance.Consumables[featureId].ResetInterval == Interval.Monthly && now.Subtract(subscriptionInstance.StartDateUtc).Days % 30 == 0)
                {
                    session.Events.Append(subscriptionInstance.Id, new ConsumableReset(featureId));
                }
            }
        }
    }

    private async Task ExecutePointCapReset(DateTimeOffset now, CancellationToken cancellationToken)
    {
        IReadOnlyList<PointBalanceDetail> pointBalances = await session.Query
            <PointBalanceDetail>().ToListAsync(cancellationToken);
        IReadOnlyList<PointCap> pointCaps = (await pointCapRepository.GetAllAsync(cancellationToken)).AsList();
        foreach (PointBalanceDetail pointBalance in pointBalances)
        {
            foreach (PointCap pointCap in pointCaps)
            {
                if (pointBalance.Categories.ContainsKey(pointCap.Category))
                {
                    if (pointCap.ResetInterval == Interval.Daily)
                    {
                        session.Events.Append(pointBalance.Id, new PointCapReset(Interval.Daily));
                    }
                    if (pointCap.ResetInterval == Interval.Weekly && now.DayOfWeek == DayOfWeek.Monday)
                    {
                        session.Events.Append(pointBalance.Id, new PointCapReset(Interval.Weekly));
                    }
                    if (pointCap.ResetInterval == Interval.Monthly && now.Day == 1)
                    {
                        session.Events.Append(pointBalance.Id, new PointCapReset(Interval.Monthly));
                    }
                }
            }
        }
    }
}
