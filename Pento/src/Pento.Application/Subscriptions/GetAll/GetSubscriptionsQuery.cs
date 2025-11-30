using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Pagination;
using Pento.Application.Subscriptions.GetById;

namespace Pento.Application.Subscriptions.GetAll;

public sealed record GetSubscriptionsQuery(string? SearchTerm, bool? IsActive, int PageNumber, int PageSize) : IQuery<PagedList<SubscriptionDetailResponse>>;
