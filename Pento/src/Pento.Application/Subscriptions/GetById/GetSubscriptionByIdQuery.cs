using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;
using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Subscriptions.GetById;

public sealed record GetSubscriptionByIdQuery(Guid SubscriptionId) : IQuery<SubscriptionDetailResponse>;
