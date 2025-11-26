using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Pagination;

namespace Pento.Application.Subscriptions.GetById;

public sealed record GetSubscriptionsQuery(string? SearchTerm, int PageNumber, int PageSize) : IQuery<PagedList<SubscriptionDetailResponse>>;
