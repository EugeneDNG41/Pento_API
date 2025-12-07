using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Payments.GetById;

public sealed record GetPaymentByIdQuery(Guid PaymentId) : IQuery<PaymentResponse>;
