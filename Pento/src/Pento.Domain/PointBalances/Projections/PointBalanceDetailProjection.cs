
using Pento.Domain.PointBalances.Events;
using Pento.Domain.PointTasks;
using Pento.Domain.Shared;

namespace Pento.Domain.PointBalances.Projections;


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

