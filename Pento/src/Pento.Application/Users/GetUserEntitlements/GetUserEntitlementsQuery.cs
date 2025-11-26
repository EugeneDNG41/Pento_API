using Pento.Application.Abstractions.Messaging;
using Pento.Application.Users.GetCurrentEntitlements;

namespace Pento.Application.Users.GetUserEntitlements;

public sealed record GetUserEntitlementsQuery(Guid UserId, string? SearchText, bool? Available) : IQuery<IReadOnlyList<UserEntitlementResponse>>;

