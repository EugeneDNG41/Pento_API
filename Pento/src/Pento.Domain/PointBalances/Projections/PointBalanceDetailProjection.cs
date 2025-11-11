
using Pento.Domain.PointBalances.Events;
using Pento.Domain.PointTasks;
using Pento.Domain.Shared;

namespace Pento.Domain.PointBalances.Projections;

public sealed class PointBalanceDetail
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public Dictionary<ActivityType, PointsByCategory> Categories { get; init; } = [];
    public int Balance { get; private set; }
    public void Add(PointAdded e)
    {
        if (!Categories.TryGetValue(e.Category, out PointsByCategory? bucket))
        {
            bucket = new PointsByCategory();
            Categories[e.Category] = bucket;
        }
        bucket.Add(e.Amount, e.CountedTowardsWeeklyCap, e.CountedTowardsMonthlyCap);
        Balance += e.Amount;
    }
    public void ResetPointCap(Period period)
    {
        if (Categories is null || Categories.Count == 0)
        {
            return;
        }
        switch (period)
        {
            case Period.Daily:
                foreach (PointsByCategory cat in Categories.Values)
                {
                    cat.DailyReset();
                }
                break;
            case Period.Weekly:
                foreach (PointsByCategory cat in Categories.Values)
                {
                    cat.WeeklyReset();
                }
                break;
            case Period.Monthly:
                foreach (PointsByCategory cat in Categories.Values)
                {
                    cat.MonthlyReset();
                }
                break;
            default:
                break;
        }
    }
}
public sealed class PointsByCategory
{
    public int Points { get; private set; }
    public int DailyEarnedPoints { get; private set; }
    public int WeeklyEarnedPoints { get; private set; }
    public int MonthlyEarnedPoints { get; private set; }
    public void Add(int amount, bool countTowardsWeeklyCap, bool countTowardsMonthlyCap)
    {
        Points += amount;
        DailyEarnedPoints += amount;
        WeeklyEarnedPoints += countTowardsWeeklyCap ? amount : 0;
        MonthlyEarnedPoints += countTowardsMonthlyCap ? amount : 0;
    }
    public void DailyReset()
    {
        DailyEarnedPoints = 0;
    }
    public void WeeklyReset()
    {
        WeeklyEarnedPoints = 0;
    }
    public void MonthlyReset()
    {
        MonthlyEarnedPoints = 0;
    }
}
internal sealed class PointBalanceDetailProjection
{
    public PointBalanceDetail Create(BalanceCreated e) => new() { Id = e.Id, UserId = e.UserId };

    public static PointBalanceDetail Apply(PointAdded e, PointBalanceDetail balance)
    {
        balance.Add(e);
        return balance;
    }
    public static PointBalanceDetail Apply(PointCapReset e, PointBalanceDetail balance)
    {
        balance.ResetPointCap(e.Period);
        return balance;
    }
}
