using Pento.Domain.Abstractions;

namespace Pento.Domain.DeviceTokens;

public sealed class DeviceToken : Entity
{
    public Guid UserId { get; private set; }
    public string Token { get; private set; }
    public DevicePlatform Platform { get; private set; }
    private DeviceToken() { }

    public DeviceToken(Guid userId, string token, DevicePlatform platform)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Token = token;
        Platform = platform;

    }
    public void ReassignTo(Guid newUserId, DevicePlatform platform)
    {
        UserId = newUserId;
        Platform = platform;
    }


    public void UpdateToken(string token)
    {
        Token = token;
    }
}

public enum DevicePlatform
{
    Android = 1,
    IOS = 2,
    Web = 3
}
