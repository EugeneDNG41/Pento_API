using NUnit.Framework;
using Pento.Domain.Subscriptions;
using Pento.Domain.Shared;

namespace Pento.Domain.UnitTests;

internal sealed class SubscriptionFeatureTests
{
    /// <summary>
    /// Ensures Create() initializes properties correctly
    /// and raises SubscriptionFeatureAddedOrUpdatedDomainEvent.
    /// </summary>
    [Test]
    public void Create_ValidInputs_InitializesPropertiesAndRaisesEvent()
    {
        // Arrange
        var subscriptionId = Guid.NewGuid();
        string featureCode = "OCR_SCAN";
        int quota = 100;
        TimeUnit resetPeriod = TimeUnit.Month;

        // Act
        var feature = SubscriptionFeature.Create(
            subscriptionId,
            featureCode,
            quota,
            resetPeriod);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(feature.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(feature.SubscriptionId, Is.EqualTo(subscriptionId));
            Assert.That(feature.FeatureCode, Is.EqualTo(featureCode));
            Assert.That(feature.Quota, Is.EqualTo(quota));
            Assert.That(feature.ResetPeriod, Is.EqualTo(resetPeriod));

            SubscriptionFeatureAddedOrUpdatedDomainEvent ev = feature.GetDomainEvents()
                .OfType<SubscriptionFeatureAddedOrUpdatedDomainEvent>()
                .Single();

            Assert.That(ev.SubscriptionFeatureId, Is.EqualTo(feature.Id));
        });
    }

    /// <summary>
    /// Ensures UpdateDetails updates all provided values
    /// and raises SubscriptionFeatureAddedOrUpdatedDomainEvent.
    /// </summary>
    [Test]
    public void UpdateDetails_ValidInputs_UpdatesFieldsAndRaisesEvent()
    {
        // Arrange
        var feature = SubscriptionFeature.Create(
            Guid.NewGuid(),
            "OLD_FEATURE",
            quota: 10,
            resetPeriod: TimeUnit.Day);


        // Act
        feature.UpdateDetails(
            featureCode: "NEW_FEATURE",
            quota: 50,
            resetPeriod: TimeUnit.Month);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(feature.FeatureCode, Is.EqualTo("NEW_FEATURE"));
            Assert.That(feature.Quota, Is.EqualTo(50));
            Assert.That(feature.ResetPeriod, Is.EqualTo(TimeUnit.Month));

            SubscriptionFeatureAddedOrUpdatedDomainEvent ev = feature.GetDomainEvents()
                .OfType<SubscriptionFeatureAddedOrUpdatedDomainEvent>()
                .Single();

            Assert.That(ev.SubscriptionFeatureId, Is.EqualTo(feature.Id));
        });
    }

    /// <summary>
    /// Ensures UpdateDetails ignores null / same values
    /// but still raises SubscriptionFeatureAddedOrUpdatedDomainEvent.
    /// </summary>
    [Test]
    public void UpdateDetails_NullOrSameValues_KeepsStateButRaisesEvent()
    {
        // Arrange
        var feature = SubscriptionFeature.Create(
            Guid.NewGuid(),
            "FEATURE",
            quota: null,
            resetPeriod: null);


        // Act
        feature.UpdateDetails(
            featureCode: null, // ignored
            quota: null,       // same
            resetPeriod: null  // same
        );

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(feature.FeatureCode, Is.EqualTo("FEATURE"));
            Assert.That(feature.Quota, Is.Null);
            Assert.That(feature.ResetPeriod, Is.Null);

            Assert.That(
                feature.GetDomainEvents()
                    .OfType<SubscriptionFeatureAddedOrUpdatedDomainEvent>()
                    .Any(),
                Is.True);
        });
    }

    /// <summary>
    /// Ensures UpdateDetails can clear quota and reset period.
    /// </summary>
    [Test]
    public void UpdateDetails_ClearQuotaAndResetPeriod_SetsNulls()
    {
        // Arrange
        var feature = SubscriptionFeature.Create(
            Guid.NewGuid(),
            "FEATURE",
            quota: 20,
            resetPeriod: TimeUnit.Week);


        // Act
        feature.UpdateDetails(
            featureCode: null,
            quota: null,
            resetPeriod: null);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(feature.Quota, Is.Null);
            Assert.That(feature.ResetPeriod, Is.Null);
        });
    }
}
