
using Pento.Domain.Abstractions;

namespace Pento.Domain.Payments;

public static class PaymentErrors
{
    public static readonly Error PaymentCreationFailed = Error.Failure(
        "PayOS.PaymentCreationFailed",
        "Failed to create payment link.");
    public static readonly Error InvalidWebhook = Error.Failure(
        "PayOS.InvalidWebhook",
        "The webhook could not be verified.");
    public static readonly Error WebhookConfirmationFailed = Error.Failure(
        "PayOS.WebhookConfirmationFailed",
        "Failed to confirm the webhook.");
    public static readonly Error PaymentCancellationFailed = Error.Failure(
        "PayOS.PaymentCancellationFailed",
        "Failed to cancel the payment.");
    public static readonly Error PaymentNotFound = Error.Failure(
        "Payment.NotFound",
        "Payment not found.");
}



