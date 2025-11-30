using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Payments.Delete;

public sealed record DeletePaymentCommand(Guid PaymentId) : ICommand;
