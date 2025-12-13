using NUnit.Framework;
using Pento.Domain.MilestoneRequirements;
using Pento.Domain.Milestones;

namespace Pento.Domain.UnitTests;

internal sealed class MilestoneRequirementTests
{
    /// <summary>
    /// Ensures Create() initializes all properties correctly.
    /// </summary>
    [Test]
    public void Create_ValidInputs_InitializesProperties()
    {
        // Arrange
        var milestoneId = Guid.NewGuid();
        string activityCode = "FOOD_CONSUMED";
        int quota = 10;
        int? withinDays = 30;

        // Act
        var req = MilestoneRequirement.Create(
            milestoneId,
            activityCode,
            quota,
            withinDays);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(req.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(req.MilestoneId, Is.EqualTo(milestoneId));
            Assert.That(req.ActivityCode, Is.EqualTo(activityCode));
            Assert.That(req.Quota, Is.EqualTo(quota));
            Assert.That(req.WithinDays, Is.EqualTo(withinDays));
        });
    }

    /// <summary>
    /// Ensures UpdateDetails updates all provided values
    /// and raises MilestoneEnabledOrUpdatedDomainEvent.
    /// </summary>
    [Test]
    public void UpdateDetails_ValidInputs_UpdatesFieldsAndRaisesEvent()
    {
        // Arrange
        var milestoneId = Guid.NewGuid();

        var req = MilestoneRequirement.Create(
            milestoneId,
            "OLD_CODE",
            5,
            7);


        // Act
        req.UpdateDetails(
            activityCode: "NEW_CODE",
            quota: 20,
            withinDays: 30);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(req.ActivityCode, Is.EqualTo("NEW_CODE"));
            Assert.That(req.Quota, Is.EqualTo(20));
            Assert.That(req.WithinDays, Is.EqualTo(30));

            MilestoneEnabledOrUpdatedDomainEvent ev = req.GetDomainEvents()
                .OfType<MilestoneEnabledOrUpdatedDomainEvent>()
                .Single();

            Assert.That(ev.MilestoneId, Is.EqualTo(milestoneId));
        });
    }

    /// <summary>
    /// Ensures UpdateDetails ignores null / unchanged values
    /// but still raises MilestoneEnabledOrUpdatedDomainEvent.
    /// </summary>
    [Test]
    public void UpdateDetails_NullOrSameValues_KeepsStateButRaisesEvent()
    {
        // Arrange
        var milestoneId = Guid.NewGuid();

        var req = MilestoneRequirement.Create(
            milestoneId,
            "CODE",
            10,
            null);


        // Act
        req.UpdateDetails(
            activityCode: null,   // ignored
            quota: 10,            // same value
            withinDays: null);    // same value

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(req.ActivityCode, Is.EqualTo("CODE"));
            Assert.That(req.Quota, Is.EqualTo(10));
            Assert.That(req.WithinDays, Is.Null);

            Assert.That(
                req.GetDomainEvents().OfType<MilestoneEnabledOrUpdatedDomainEvent>().Any(),
                Is.True);
        });
    }

    /// <summary>
    /// Ensures UpdateDetails can clear WithinDays when set to null.
    /// </summary>
    [Test]
    public void UpdateDetails_ClearWithinDays_SetsNull()
    {
        // Arrange
        var req = MilestoneRequirement.Create(
            Guid.NewGuid(),
            "CODE",
            3,
            14);


        // Act
        req.UpdateDetails(
            activityCode: null,
            quota: null,
            withinDays: null);

        // Assert
        Assert.That(req.WithinDays, Is.Null);
    }
}
