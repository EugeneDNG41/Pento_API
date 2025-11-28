
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
    public static readonly Error PaymentStatusRetrievalFailed = Error.Failure(
        "PayOS.PaymentStatusRetrievalFailed",
        "Failed to retrieve payment status from PayOS.");
    public static readonly Error NotFound = Error.Failure(
        "Payment.NotFound",
        "Payment not found.");
    public static readonly Error ForbiddenAccess = Error.Forbidden(
        "Payment.ForbiddenAccess",
        "You do not have permission to access this payment.");
    public static readonly Error PendingOrProcessingPayment = Error.Conflict(
        "Payment.PendingOrProcessingPayment",
        "You already have a pending or processing payment for this subscription plan."); //business rule
}



