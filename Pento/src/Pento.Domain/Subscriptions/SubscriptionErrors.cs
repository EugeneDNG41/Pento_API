using Pento.Domain.Abstractions;

namespace Pento.Domain.Subscriptions;

public static class SubscriptionErrors
{
    public static readonly Error SubscriptionNotFound =
        Error.NotFound("Subscription.NotFound", "Subscription not found.");
    public static readonly Error SubscriptionPlanNotFound =
        Error.NotFound("Subscription.PlanNotFound", "Subscription plan not found.");
    public static readonly Error SubscriptionFeatureNotFound =
        Error.NotFound("Subscription.FeatureNotFound", "Subscription feature not found.");
    public static readonly Error UserSubscriptionNotFound =
        Error.NotFound("Subscription.UserSubscriptionNotFound", "User subscription not found.");
    public static readonly Error NameTaken =
        Error.Conflict("Subscription.NameTaken", "Subscription name already taken.");
    public static readonly Error DuplicateSubscriptionPlan =
        Error.Conflict("Subscription.DuplicateSubscriptionPlan", "A subscription plan with the same price and duration already exists for this subscription."); //business rule
    public static readonly Error DuplicateSubscriptionFeature =
        Error.Conflict("Subscription.DuplicateSubscriptionFeature", "This feature has already been added to the subscription."); //business rule
    public static readonly Error InactiveSubscription =
        Error.Conflict("Subscription.InactiveSubscription", "Cannot purchase an inactive subscription.");
    public static readonly Error SubscriptionInUse =
        Error.Conflict("Subscription.InUse", "Cannot delete subscription as it is associated with active users."); //business rule
    public static readonly Error CannotPauseLifetimeSubscription =
        Error.Conflict("Subscription.CannotPauseLifetimeSubscription", "Cannot pause a lifetime subscription."); //business rule
    public static readonly Error NoMoreThanOneLifetimePlan =
        Error.Conflict("Subscription.NoMoreThanOneLifetimePlan", "A subscription can have only one lifetime plan."); //business rule
    public static readonly Error ForbiddenAccess =
        Error.Forbidden("Subscription.ForbiddenAccess", "You do not have permission to access this user subscription.");
    public static readonly Error SubscriptionExpired =
        Error.Conflict("Subscription.SubscriptionExpired", "The subscription has expired.");
    public static readonly Error SubscriptionPaused =
        Error.Conflict("Subscription.SubscriptionPaused", "The subscription is still on pause.");
    public static readonly Error SubscriptionActive =
        Error.Conflict("Subscription.SubscriptionActive", "The subscription is still active.");
    public static readonly Error SubscriptionCancelled =
        Error.Conflict("Subscription.SubscriptionCancelled", "The subscription has been cancelled.");
    public static Error MinimumPauseDay(int days) =>
        Error.Conflict("Subscription.MinimumPauseDay", $"The subscription must have been paused for at least {days} day(s).");
    public static readonly Error CannotExtendLifetimeSubscription =
        Error.Conflict("Subscription.CannotExtendLifetimeSubscription", "Cannot extend a lifetime subscription."); //business rule
    public static readonly Error CannotReduceBelowRemainingDay =
        Error.Conflict("Subscription.CannotReduceBelowRemainingDay", "Cannot reduce subscription duration below remaining day."); //business rule
}

