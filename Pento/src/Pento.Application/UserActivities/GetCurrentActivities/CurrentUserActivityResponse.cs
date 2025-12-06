namespace Pento.Application.UserActivities.GetCurrentActivities;

public sealed record CurrentUserActivityResponse
{
    public string Name { get; init; }
    public string Description { get; init; }
    public DateTime PerformedON { get; init; }
}
