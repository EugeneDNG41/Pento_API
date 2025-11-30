using Pento.Application.Abstractions.Messaging;
using Pento.Application.UserEntitlements.GetCurrentEntitlements;

namespace Pento.Application.UserEntitlements.GetUserEntitlements;

public sealed record GetUserEntitlementsQuery(Guid UserId, string? SearchText, bool? Available) : IQuery<IReadOnlyList<UserEntitlementResponse>>;

