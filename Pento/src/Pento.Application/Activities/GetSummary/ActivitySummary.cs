namespace Pento.Application.Activities.GetSummary;

public sealed record ActivitySummary
{
    public string Code { get; init; }
    public string Name { get; init; }
    public int TotalCount => CountByDate.Sum(c => c.Count);
    public List<ActivityCountByDate> CountByDate { get; init; } = new();  
}
public sealed record ActivityCountByDate
{
    public DateOnly Date { get; init; }
    public int Count { get; init; }

}
