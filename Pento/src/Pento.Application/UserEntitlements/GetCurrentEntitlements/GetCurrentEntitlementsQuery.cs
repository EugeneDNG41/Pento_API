using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.UserEntitlements.GetCurrentEntitlements;

public sealed record GetCurrentEntitlementsQuery(string? SearchText, bool? Available) : IQuery<IReadOnlyList<UserEntitlementResponse>>;

