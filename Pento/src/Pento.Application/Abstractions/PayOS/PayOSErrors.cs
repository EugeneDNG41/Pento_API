using Pento.Domain.Abstractions;

namespace Pento.Application.Abstractions.PayOS;

public static class PayOSErrors
{
    public static  Error PaymentCreationFailed(string message) => Error.Failure(
        "PayOS.PaymentCreationFailed",
        $"Failed to create payment link due to {message}.");
    public static readonly Error InvalidWebhook  = Error.Failure(
        "PayOS.InvalidWebhook",
        "The webhook could not be verified.");
    public static readonly Error WebhookConfirmationFailed = Error.Failure(
        "PayOS.WebhookConfirmationFailed",
        "Failed to confirm the webhook.");
}
