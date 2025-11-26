using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Users.GetCurrentEntitlements;

public sealed record GetCurrentEntitlementsQuery(string? SearchText, bool? Available) : IQuery<IReadOnlyList<UserEntitlementResponse>>;

