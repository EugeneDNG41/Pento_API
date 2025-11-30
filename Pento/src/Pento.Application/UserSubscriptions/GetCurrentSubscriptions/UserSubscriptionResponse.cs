namespace Pento.Application.UserSubscriptions.GetCurrentSubscriptions;

public sealed record UserSubscriptionResponse
{
    public Guid UserSubscriptionId { get; init; }
    public Guid SubscriptionId { get; init; }
    public string SubscriptionName { get; init; }
    public string Status { get; init; }
    public DateOnly StartDate { get; init; }
    public DateOnly? EndDate { get; init; }
    public DateOnly? PausedDate { get; init; }
    public DateOnly? CancelledDate {  get; init; }
    public string Duration { get; init; }
}
