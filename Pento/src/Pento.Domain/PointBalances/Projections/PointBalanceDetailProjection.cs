using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marten.Events.Aggregation;
using Pento.Domain.PointBalances.Events;
using Pento.Domain.Shared;

namespace Pento.Domain.PointBalances.Projections;

public sealed record PointBalanceDetail
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public int Balance => PointsByCategory.Sum(p => p.Points);
    public List<PointsByCategory> PointsByCategory { get; init; }
}
public sealed record PointsByCategory
{
    public PointCategory CategoryName { get; init; }
    public int Points { get; init; }
    public int DailyEarnedPoints { get; init; }
    public int WeeklyEarnedPoints { get; init; }
    public int MonthlyEarnedPoints { get; init; }
}
internal sealed class PointBalanceDetailProjection : SingleStreamProjection<PointBalanceDetail, Guid>
{
    public PointBalanceDetail Create(BalanceCreated e)
        => new()
        {
            Id = e.Id,
            UserId = e.UserId,
            PointsByCategory = []
        };
    public PointBalanceDetail Apply(PointAdded e, PointBalanceDetail balance)
    {
        PointsByCategory? pointsByCategory = balance.PointsByCategory.FirstOrDefault(p => p.CategoryName == e.Category);
        if (pointsByCategory is null)
        {
            pointsByCategory = new PointsByCategory
            {
                CategoryName = e.Category,
                Points = e.Amount,
                DailyEarnedPoints = e.Amount,
                WeeklyEarnedPoints = e.Amount,
                MonthlyEarnedPoints = e.Amount
            };
            balance.PointsByCategory.Add(pointsByCategory);
        }
        else
        {
            balance.PointsByCategory.Remove(pointsByCategory);
            pointsByCategory = pointsByCategory with
            {
                Points = pointsByCategory.Points + e.Amount,
                DailyEarnedPoints = pointsByCategory.DailyEarnedPoints + e.Amount,
                WeeklyEarnedPoints = pointsByCategory.WeeklyEarnedPoints + e.Amount,
                MonthlyEarnedPoints = pointsByCategory.MonthlyEarnedPoints + e.Amount
            };
            balance.PointsByCategory.Add(pointsByCategory);
        }
        return balance;
    }
}
