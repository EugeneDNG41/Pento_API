using NUnit.Framework;
using Pento.Domain.Subscriptions;
using Pento.Domain.Shared;

namespace Pento.Domain.UnitTests;

internal sealed class SubscriptionPlanTests
{
    /// <summary>
    /// Ensures Create() initializes all properties correctly.
    /// </summary>
    [Test]
    public void Create_ValidInputs_InitializesProperties()
    {
        // Arrange
        var subscriptionId = Guid.NewGuid();
        long amount = 199_000;
        Currency currency = Currency.VND;
        int? duration = 30;

        // Act
        var plan = SubscriptionPlan.Create(
            subscriptionId,
            amount,
            currency,
            duration);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(plan.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(plan.SubscriptionId, Is.EqualTo(subscriptionId));
            Assert.That(plan.Amount, Is.EqualTo(amount));
            Assert.That(plan.Currency, Is.EqualTo(currency));
            Assert.That(plan.DurationInDays, Is.EqualTo(duration));
        });
    }

   

    /// <summary>
    /// Ensures UpdateDetails ignores null values
    /// and keeps existing state.
    /// </summary>
    [Test]
    public void UpdateDetails_NullInputs_KeepsExistingValues()
    {
        // Arrange
        var plan = SubscriptionPlan.Create(
            Guid.NewGuid(),
            199_000,
            Currency.VND,
            30);

        // Act
        plan.UpdateDetails(
            amount: null,
            currency: null,
            duration: null);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(plan.Amount, Is.EqualTo(199_000));
            Assert.That(plan.Currency, Is.EqualTo(Currency.VND));
            Assert.That(plan.DurationInDays, Is.Null);
        });
    }

    /// <summary>
    /// Ensures UpdateDetails can clear DurationInDays.
    /// </summary>
    [Test]
    public void UpdateDetails_ClearDuration_SetsNull()
    {
        // Arrange
        var plan = SubscriptionPlan.Create(
            Guid.NewGuid(),
            299_000,
            Currency.VND,
            90);

        // Act
        plan.UpdateDetails(
            amount: null,
            currency: null,
            duration: null);

        // Assert
        Assert.That(plan.DurationInDays, Is.Null);
    }

  
}
