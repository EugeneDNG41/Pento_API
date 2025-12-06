namespace Pento.Application.UserActivities.GetUserActivities;

public sealed record UserActivityResponse
{
    public Guid UserId { get; init; }
    public string Name { get; init; } 
    public string Description { get; init; }
    public DateTime PerformedON { get; init; }
    public Guid? EntityId { get; init; }
}
