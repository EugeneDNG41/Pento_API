using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;

namespace Pento.Domain.Payments;


public class PaymentCompletedDomainEvent(Guid userId, Guid subscriptionPlanId) : DomainEvent
{
    public Guid UserId { get; } = userId;
    public Guid SubscriptionPlanId { get; } = subscriptionPlanId;
}

