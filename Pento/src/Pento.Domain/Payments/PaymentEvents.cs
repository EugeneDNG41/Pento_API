using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;

namespace Pento.Domain.Payments;


public class PaymentCompletedDomainEvent(Guid paymentId, Guid subscriptionPlanId) : DomainEvent
{
    public Guid PaymentId { get; } = paymentId;
    public Guid SubscriptionPlanId { get; } = subscriptionPlanId;
}

