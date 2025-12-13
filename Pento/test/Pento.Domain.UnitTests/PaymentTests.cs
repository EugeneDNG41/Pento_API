using NUnit.Framework;
using Pento.Domain.Payments;
using Pento.Domain.Shared;

namespace Pento.Domain.UnitTests;

internal sealed class PaymentTests
{
    private static Payment CreateSample(DateTime now)
    {
        return Payment.Create(
            userId: Guid.NewGuid(),
            subscriptionPlanId: Guid.NewGuid(),
            paymentLinkId: null,
            description: "Monthly subscription",
            amountDue: 100_000,
            amountPaid: 0,
            currency: Currency.VND,
            checkoutUrl: new Uri("https://pay.example/checkout"),
            qrCode: "QR123",
            createdAt: now
        );
    }

    /// <summary>
    /// Ensures Create() initializes properties and sets Pending status.
    /// </summary>
    [Test]
    public void Create_ValidInputs_InitializesProperties()
    {
        // Arrange
        DateTime now = DateTime.UtcNow;

        // Act
        Payment payment = CreateSample(now);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(payment.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(payment.Status, Is.EqualTo(PaymentStatus.Pending));
            Assert.That(payment.AmountDue, Is.EqualTo(100_000));
            Assert.That(payment.AmountPaid, Is.EqualTo(0));
            Assert.That(payment.Currency, Is.EqualTo(Currency.VND));
            Assert.That(payment.CreatedAt, Is.EqualTo(now));
            Assert.That(payment.CheckoutUrl, Is.Not.Null);
            Assert.That(payment.QrCode, Is.Not.Null);
        });
    }

    /// <summary>
    /// Ensures UpdatePaymentLink updates link-related fields.
    /// </summary>
    [Test]
    public void UpdatePaymentLink_UpdatesFields()
    {
        // Arrange
        Payment payment = CreateSample(DateTime.UtcNow);
        DateTime expiresAt = DateTime.UtcNow.AddMinutes(15);
        var newCheckout = new Uri("https://pay.example/new");

        // Act
        payment.UpdatePaymentLink(
            paymentLinkId: "plink_123",
            providerDescription: "PayOS",
            checkoutUrl: newCheckout,
            qrCode: "QR_NEW",
            expiresAt: expiresAt
        );

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(payment.PaymentLinkId, Is.EqualTo("plink_123"));
            Assert.That(payment.ProviderDescription, Is.EqualTo("PayOS"));
            Assert.That(payment.CheckoutUrl, Is.EqualTo(newCheckout));
            Assert.That(payment.QrCode, Is.EqualTo("QR_NEW"));
            Assert.That(payment.ExpiresAt, Is.EqualTo(expiresAt));
        });
    }

    /// <summary>
    /// Ensures MarkAsPaid updates status, clears checkout data,
    /// sets PaidAt and raises PaymentCompletedDomainEvent.
    /// </summary>
    [Test]
    public void MarkAsPaid_SetsPaidAndRaisesEvent()
    {
        // Arrange
        Payment payment = CreateSample(DateTime.UtcNow);
        DateTime paidAt = DateTime.UtcNow.AddMinutes(2);

        // Act
        payment.MarkAsPaid(amountPaid: 100_000, paidAt: paidAt);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(payment.Status, Is.EqualTo(PaymentStatus.Paid));
            Assert.That(payment.AmountPaid, Is.EqualTo(100_000));
            Assert.That(payment.PaidAt, Is.EqualTo(paidAt));
            Assert.That(payment.CheckoutUrl, Is.Null);
            Assert.That(payment.QrCode, Is.Null);

            PaymentCompletedDomainEvent ev = payment.GetDomainEvents()
                .OfType<PaymentCompletedDomainEvent>()
                .Single();

            Assert.That(ev.PaymentId, Is.EqualTo(payment.Id));
        });
    }

    /// <summary>
    /// Ensures MarkAsCancelled sets status, reason, time and clears checkout data.
    /// </summary>
    [Test]
    public void MarkAsCancelled_SetsCancelledState()
    {
        // Arrange
        Payment payment = CreateSample(DateTime.UtcNow);
        DateTime cancelledAt = DateTime.UtcNow;

        // Act
        payment.MarkAsCancelled("User cancelled", cancelledAt);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(payment.Status, Is.EqualTo(PaymentStatus.Cancelled));
            Assert.That(payment.CancellationReason, Is.EqualTo("User cancelled"));
            Assert.That(payment.CancelledAt, Is.EqualTo(cancelledAt));
            Assert.That(payment.CheckoutUrl, Is.Null);
            Assert.That(payment.QrCode, Is.Null);
        });
    }

    /// <summary>
    /// Ensures MarkAsFailed sets status and clears checkout data.
    /// </summary>
    [Test]
    public void MarkAsFailed_SetsFailedState()
    {
        Payment payment = CreateSample(DateTime.UtcNow);

        payment.MarkAsFailed();

        Assert.Multiple(() =>
        {
            Assert.That(payment.Status, Is.EqualTo(PaymentStatus.Failed));
            Assert.That(payment.CheckoutUrl, Is.Null);
            Assert.That(payment.QrCode, Is.Null);
        });
    }

    /// <summary>
    /// Ensures MarkAsProcessing sets status only.
    /// </summary>
    [Test]
    public void MarkAsProcessing_SetsProcessingStatus()
    {
        Payment payment = CreateSample(DateTime.UtcNow);

        payment.MarkAsProcessing();

        Assert.That(payment.Status, Is.EqualTo(PaymentStatus.Processing));
    }

    /// <summary>
    /// Ensures MarkAsExpired sets status and clears checkout data.
    /// </summary>
    [Test]
    public void MarkAsExpired_SetsExpiredState()
    {
        Payment payment = CreateSample(DateTime.UtcNow);

        payment.MarkAsExpired();

        Assert.Multiple(() =>
        {
            Assert.That(payment.Status, Is.EqualTo(PaymentStatus.Expired));
            Assert.That(payment.CheckoutUrl, Is.Null);
            Assert.That(payment.QrCode, Is.Null);
        });
    }
}
