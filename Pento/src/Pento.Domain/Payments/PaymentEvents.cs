using Pento.Domain.Abstractions;

namespace Pento.Domain.Payments;


public sealed class PaymentCompletedDomainEvent(Guid paymentId) : DomainEvent
{
    public Guid PaymentId { get; } = paymentId;
}

