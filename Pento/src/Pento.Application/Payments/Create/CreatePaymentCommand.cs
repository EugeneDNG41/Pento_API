using Pento.Application.Abstractions.External.PayOS;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Shared;

namespace Pento.Application.Payments.Create;

public sealed record CreatePaymentCommand(Guid SubscriptionPlanId) : ICommand<PaymentLinkResponse>;
