using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Payments.Cancel;

public sealed record CancelPaymentCommand(Guid PaymentId, string? Reason) : ICommand;
