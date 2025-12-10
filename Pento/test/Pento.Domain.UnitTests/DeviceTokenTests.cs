using NUnit.Framework;
using Pento.Domain.DeviceTokens;

namespace Pento.Domain.UnitTests;

internal sealed class DeviceTokenTests
{
    /// <summary>
    /// Verifies that constructor initializes all properties correctly.
    /// </summary>
    [Test]
    public void Constructor_ValidInputs_InitializesProperties()
    {
        // Arrange
        var userId = Guid.NewGuid();
        string token = "abc123";
        DevicePlatform platform = DevicePlatform.Android;

        // Act
        var deviceToken = new DeviceToken(userId, token, platform);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(deviceToken.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(deviceToken.UserId, Is.EqualTo(userId));
            Assert.That(deviceToken.Token, Is.EqualTo(token));
            Assert.That(deviceToken.Platform, Is.EqualTo(platform));
        });
    }

    /// <summary>
    /// Verifies that ReassignTo updates UserId and Platform.
    /// </summary>
    [Test]
    public void ReassignTo_ValidInputs_UpdatesUserIdAndPlatform()
    {
        // Arrange
        var deviceToken = new DeviceToken(
            Guid.NewGuid(),
            "originalToken",
            DevicePlatform.Android);

        var newUserId = Guid.NewGuid();
        DevicePlatform newPlatform = DevicePlatform.IOS;

        // Act
        deviceToken.ReassignTo(newUserId, newPlatform);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(deviceToken.UserId, Is.EqualTo(newUserId));
            Assert.That(deviceToken.Platform, Is.EqualTo(newPlatform));
        });
    }

    /// <summary>
    /// Verifies that UpdateToken sets the new token value.
    /// </summary>
    [Test]
    public void UpdateToken_ValidInput_UpdatesToken()
    {
        // Arrange
        var deviceToken = new DeviceToken(
            Guid.NewGuid(),
            "oldToken",
            DevicePlatform.Web);

        string newToken = "updatedToken";

        // Act
        deviceToken.UpdateToken(newToken);

        // Assert
        Assert.That(deviceToken.Token, Is.EqualTo(newToken));
    }
}
