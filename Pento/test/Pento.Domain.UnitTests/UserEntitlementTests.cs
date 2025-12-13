using NUnit.Framework;
using Pento.Domain.UserEntitlements;
using Pento.Domain.Shared;

namespace Pento.Domain.UnitTests;

internal sealed class UserEntitlementTests
{
    /// <summary>
    /// Ensures Create initializes all properties correctly.
    /// </summary>
    [Test]
    public void Create_ValidInputs_InitializesProperties()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var subscriptionId = Guid.NewGuid();
        string featureCode = "OCR_SCAN";
        int quota = 100;
        TimeUnit resetPeriod = TimeUnit.Month;

        // Act
        var entitlement = UserEntitlement.Create(
            userId,
            subscriptionId,
            featureCode,
            quota,
            resetPeriod);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(entitlement.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(entitlement.UserId, Is.EqualTo(userId));
            Assert.That(entitlement.UserSubscriptionId, Is.EqualTo(subscriptionId));
            Assert.That(entitlement.FeatureCode, Is.EqualTo(featureCode));
            Assert.That(entitlement.Quota, Is.EqualTo(quota));
            Assert.That(entitlement.ResetPeriod, Is.EqualTo(resetPeriod));
            Assert.That(entitlement.UsageCount, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// Ensures IncrementUsage increases usage by 1 by default.
    /// </summary>
    [Test]
    public void IncrementUsage_Default_IncreasesByOne()
    {
        // Arrange
        var entitlement = UserEntitlement.Create(
            Guid.NewGuid(),
            null,
            "FEATURE");

        // Act
        entitlement.IncrementUsage();

        // Assert
        Assert.That(entitlement.UsageCount, Is.EqualTo(1));
    }

    /// <summary>
    /// Ensures IncrementUsage increases usage by specified amount.
    /// </summary>
    [Test]
    public void IncrementUsage_WithAmount_IncreasesByAmount()
    {
        // Arrange
        var entitlement = UserEntitlement.Create(
            Guid.NewGuid(),
            null,
            "FEATURE");

        // Act
        entitlement.IncrementUsage(5);

        // Assert
        Assert.That(entitlement.UsageCount, Is.EqualTo(5));
    }

    /// <summary>
    /// Ensures ResetUsage resets usage count to zero.
    /// </summary>
    [Test]
    public void ResetUsage_ResetsUsageCount()
    {
        // Arrange
        var entitlement = UserEntitlement.Create(
            Guid.NewGuid(),
            null,
            "FEATURE");

        entitlement.IncrementUsage(10);

        // Act
        entitlement.ResetUsage();

        // Assert
        Assert.That(entitlement.UsageCount, Is.EqualTo(0));
    }

    /// <summary>
    /// Ensures UpdateEntitlement updates quota, reset period and subscription id.
    /// </summary>
    [Test]
    public void UpdateEntitlement_ValidInputs_UpdatesFields()
    {
        // Arrange
        var entitlement = UserEntitlement.Create(
            Guid.NewGuid(),
            null,
            "FEATURE",
            quota: 10,
            resetPeriod: TimeUnit.Day);

        var newSubscriptionId = Guid.NewGuid();

        // Act
        entitlement.UpdateEntitlement(
            quota: 50,
            resetPeriod: TimeUnit.Month,
            userSubscriptionId: newSubscriptionId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(entitlement.Quota, Is.EqualTo(50));
            Assert.That(entitlement.ResetPeriod, Is.EqualTo(TimeUnit.Month));
            Assert.That(entitlement.UserSubscriptionId, Is.EqualTo(newSubscriptionId));
        });
    }

    /// <summary>
    /// Ensures UpdateEntitlement allows clearing quota and reset period.
    /// </summary>
    [Test]
    public void UpdateEntitlement_ClearQuotaAndResetPeriod_SetsNull()
    {
        // Arrange
        var entitlement = UserEntitlement.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "FEATURE",
            quota: 20,
            resetPeriod: TimeUnit.Week);

        // Act
        entitlement.UpdateEntitlement(
            quota: null,
            resetPeriod: null,
            userSubscriptionId: null);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(entitlement.Quota, Is.Null);
            Assert.That(entitlement.ResetPeriod, Is.Null);
            Assert.That(entitlement.UserSubscriptionId, Is.Null);
        });
    }
}
