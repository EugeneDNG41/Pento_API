using Pento.Application.Abstractions.External.PayOS;
using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Payments.Create;

public sealed record CreatePaymentCommand(Guid SubscriptionPlanId) : ICommand<PaymentLinkResponse>;
