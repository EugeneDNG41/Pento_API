using Pento.Domain.Abstractions;

namespace Pento.Domain.Payments;


public class PaymentCompletedDomainEvent(Guid paymentId) : DomainEvent
{
    public Guid PaymentId { get; } = paymentId;
}

