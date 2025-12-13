using NUnit.Framework;
using Pento.Domain.Households;

namespace Pento.Domain.UnitTests;

internal sealed class HouseholdTests
{
    /// <summary>
    /// Ensures constructor initializes properties correctly.
    /// </summary>
    [Test]
    public void Constructor_ValidInputs_InitializesProperties()
    {
        // Arrange
        var id = Guid.NewGuid();
        DateTime now = DateTime.UtcNow;

        // Act
        var household = new Household(id, "My Household", now);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(household.Id, Is.EqualTo(id));
            Assert.That(household.Name, Is.EqualTo("My Household"));
            Assert.That(household.CreatedOn, Is.EqualTo(now));
            Assert.That(household.InviteCode, Is.Null);
            Assert.That(household.InviteCodeExpirationUtc, Is.Null);
        });
    }

    /// <summary>
    /// Ensures Create() initializes household, generates invite code,
    /// and raises HouseholdCreatedDomainEvent.
    /// </summary>
    [Test]
    public void Create_ValidInputs_GeneratesInviteCodeAndRaisesEvent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        DateTime now = DateTime.UtcNow;

        // Act
        var household = Household.Create("Family", now, userId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(household.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(household.Name, Is.EqualTo("Family"));
            Assert.That(household.CreatedOn, Is.EqualTo(now));
            Assert.That(household.InviteCode, Is.Not.Null.And.Not.Empty);

            HouseholdCreatedDomainEvent ev = household.GetDomainEvents()
                .OfType<HouseholdCreatedDomainEvent>()
                .Single();

            Assert.That(ev.HouseholdId, Is.EqualTo(household.Id));
            Assert.That(ev.UserId, Is.EqualTo(userId));
        });
    }

    /// <summary>
    /// Ensures Update() changes household name.
    /// </summary>
    [Test]
    public void Update_ValidName_UpdatesName()
    {
        var household = new Household(Guid.NewGuid(), "Old Name", DateTime.UtcNow);

        household.Update("New Name");

        Assert.That(household.Name, Is.EqualTo("New Name"));
    }

    /// <summary>
    /// Ensures GenerateInviteCode sets a non-empty invite code.
    /// </summary>
    [Test]
    public void GenerateInviteCode_SetsInviteCode()
    {
        var household = new Household(Guid.NewGuid(), "House", DateTime.UtcNow);

        household.GenerateInviteCode();

        Assert.That(household.InviteCode, Is.Not.Null.And.Not.Empty);
    }

    /// <summary>
    /// Ensures RevokeInviteCode clears invite code and expiration.
    /// </summary>
    [Test]
    public void RevokeInviteCode_ClearsInviteCodeAndExpiration()
    {
        var household = Household.Create("House", DateTime.UtcNow, Guid.NewGuid());
        household.SetInviteCodeExpiration(DateTime.UtcNow.AddDays(1));

        household.RevokeInviteCode();

        Assert.Multiple(() =>
        {
            Assert.That(household.InviteCode, Is.Null);
            Assert.That(household.InviteCodeExpirationUtc, Is.Null);
        });
    }

    /// <summary>
    /// Ensures SetInviteCodeExpiration sets expiration correctly.
    /// </summary>
    [Test]
    public void SetInviteCodeExpiration_SetsExpiration()
    {
        var household = new Household(Guid.NewGuid(), "House", DateTime.UtcNow);
        DateTime expiration = DateTime.UtcNow.AddHours(2);

        household.SetInviteCodeExpiration(expiration);

        Assert.That(household.InviteCodeExpirationUtc, Is.EqualTo(expiration));
    }

    /// <summary>
    /// Ensures Delete() marks ex`ntity deleted and raises HouseholdDeletedDomainEvent.
    /// </summary>
    [Test]
    public void Delete_RaisesHouseholdDeletedEvent()
    {
        var household = Household.Create("House", DateTime.UtcNow, Guid.NewGuid());

        household.Delete();

        Assert.That(
            household.GetDomainEvents().OfType<HouseholdDeletedDomainEvent>().Any(),
            Is.True);
    }
}
