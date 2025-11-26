using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Pagination;

namespace Pento.Application.Subscriptions.GetById;

public sealed record GetSubscriptionByIdQuery(Guid SubscriptionId) : IQuery<SubscriptionDetailResponse>;
public sealed record GetSubscriptionsQuery(string? SearchTerm, int PageNumber, int PageSize) : IQuery<PagedList<SubscriptionDetailResponse>>;
