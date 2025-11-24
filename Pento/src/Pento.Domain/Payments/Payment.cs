
using Pento.Domain.Abstractions;

namespace Pento.Domain.Payments;

public class Payment : Entity
{
    private Payment() { }
    public Payment(
        Guid id,
        Guid userId,
        string? paymentLinkId,
        string description,
        long amount,
        string currency,
        PaymentStatus status,
        Uri? checkoutUrl,
        string? qrCode,
        DateTime createdAt) : base(id)
    {
        UserId = userId;
        PaymentLinkId = paymentLinkId;
        Description = description;
        Amount = amount;
        Currency = currency;
        Status = status;
        CheckoutUrl = checkoutUrl;
        QrCode = qrCode;
        CreatedAt = createdAt;
    }
    public Guid UserId { get; private set; }
    public long OrderCode { get; private set; }
    public string? PaymentLinkId { get; private set; }
    public string Description { get; private set; }
    public long Amount { get; private set; }
    public long AmountPaid { get; private set; }
    public string Currency { get; private set; }
    public PaymentStatus Status { get; private set; }
    public Uri? CheckoutUrl { get; private set; }
    public string? QrCode { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ExpiresAt { get; private set; }
    public DateTime? PaidAt { get; private set; }
    public DateTime? CancelledAt { get; private set; }   
    public string? CancellationReason { get; private set; }

    public static Payment Create(
        Guid userId,
        string? paymentLinkId,
        string description,
        long amount,
        string currency,
        Uri? checkoutUrl,
        string? qrCode,
        DateTime createdAt)
    {
        return new Payment(
            Guid.CreateVersion7(),
            userId,
            paymentLinkId,
            description,
            amount,
            currency,
            PaymentStatus.Pending,
            checkoutUrl,
            qrCode,
            createdAt);
    }

    public void UpdatePaymentLink(string paymentLinkId, Uri checkoutUrl, string qrCode, DateTime expiresAt)
    {
        PaymentLinkId = paymentLinkId;
        CheckoutUrl = checkoutUrl;
        QrCode = qrCode;
        ExpiresAt = expiresAt;
    }
    public void MarkAsPaid(long amountPaid, DateTime paidAt)
    {
        AmountPaid = amountPaid;
        Status = PaymentStatus.Paid;
        PaidAt = paidAt;
        CheckoutUrl = null;
        QrCode = null;
    }
    public void MarkAsCancelled(string? reason, DateTime cancelledAt)
    {
        Status = PaymentStatus.Cancelled;
        CancellationReason = reason;
        CancelledAt = cancelledAt;
        CheckoutUrl = null;
        QrCode = null;
    }
    public void MarkAsFailed()
    {
        Status = PaymentStatus.Failed;
        CheckoutUrl = null;
        QrCode = null;
    }
    public void MarkAsProcessing()
    {
        Status = PaymentStatus.Processing;
    }
    public void MarkAsExpired()
    {
        Status = PaymentStatus.Expired;
        CheckoutUrl = null;
        QrCode = null;
    }
}
public enum PaymentStatus
{
    Pending,
    Cancelled,
    Paid,
    Expired,
    Processing,
    Failed
}
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



