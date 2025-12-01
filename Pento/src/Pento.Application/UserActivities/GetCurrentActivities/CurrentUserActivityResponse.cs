namespace Pento.Application.UserActivities.GetCurrentActivities;

public sealed record CurrentUserActivityResponse
{
    public string Name { get; init; }
    public DateTime PerformedON { get; init; }
}
