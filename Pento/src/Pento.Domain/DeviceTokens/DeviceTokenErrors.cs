using Pento.Domain.Abstractions;

namespace Pento.Domain.DeviceTokens;

public static class DeviceTokenErrors
{
    public static readonly Error NotFound = Error.NotFound(
       "DeviceToken.NotFound",
       "The device token was not found."
   );
    public static readonly Error DuplicateToken = Error.Conflict(
       "DeviceToken.DuplicateToken",
       "The same device token already exists for this user."
        );

}
