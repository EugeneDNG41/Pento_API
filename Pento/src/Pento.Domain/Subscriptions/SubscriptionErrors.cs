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
    public static readonly Error CannotAddTimedPlanToSubscriptionWithLifetimeFeatures =
        Error.Conflict("Subscription.TimedPlanToLifetimeFeature", "Cannot add a timed subscription plan to a subscription that has lifetime features."); //business rule
    public static readonly Error CannotAddLifetimeFeatureToSubscriptionWithTimedPlans =
        Error.Conflict("Subscription.LifetimeFeatureToTimedPlan", "Cannot add a lifetime feature to a subscription that has timed plans."); //business rule
    public static readonly Error InactiveSubscription =
        Error.Conflict("Subscription.InactiveSubscription", "Cannot purchase an inactive subscription.");
    public static readonly Error SubscriptionInUse =
        Error.Conflict("Subscription.InUse", "Cannot delete subscription as it is associated with active users."); //business rule
    public static readonly Error CannotPauseLifetimeSubscription =
        Error.Conflict("Subscription.CannotPauseLifetimeSubscription", "Cannot pause a lifetime subscription."); //business rule
    public static readonly Error NoMoreThanOneLifetimePlan =
        Error.Conflict("Subscription.NoMoreThanOneLifetimePlan", "A subscription can have only one lifetime plan."); //business rule
}

