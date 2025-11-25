
using Pento.Domain.Abstractions;
using Pento.Domain.Shared;

namespace Pento.Domain.Payments;

public class Payment : Entity
{
    private Payment() { }
    public Payment(
        Guid id,
        Guid userId,
        Guid subscriptionPlanId,
        string? paymentLinkId,
        string description,
        long amountDue,
        long amountPaid,
        Currency currency,
        PaymentStatus status,
        Uri? checkoutUrl,
        string? qrCode,
        DateTime createdAt) : base(id)
    {
        UserId = userId;
        SubscriptionPlanId = subscriptionPlanId;
        PaymentLinkId = paymentLinkId;
        Description = description;
        AmountDue = amountDue;
        AmountPaid = amountPaid;
        Currency = currency;
        Status = status;
        CheckoutUrl = checkoutUrl;
        QrCode = qrCode;
        CreatedAt = createdAt;
    }
    public Guid UserId { get; private set; }
    public Guid SubscriptionPlanId { get; private set; }
    public long OrderCode { get; private set; }
    public string? PaymentLinkId { get; private set; }
    public string Description { get; private set; }
    public long AmountDue { get; private set; }
    public long AmountPaid { get; private set; }
    public Currency Currency {  get; private set; }
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
        Guid subscriptionPlanId,
        string? paymentLinkId,
        string description,
        long amountDue,
        long amountPaid,
        Currency currency,
        Uri? checkoutUrl,
        string? qrCode,
        DateTime createdAt)
    {
        return new Payment(
            Guid.CreateVersion7(),
            userId,
            subscriptionPlanId,
            paymentLinkId,
            description,
            amountDue,
            amountPaid,
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



