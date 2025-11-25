namespace Pento.Application.Payments.GetById;

public sealed record PaymentResponse(
    Guid PaymentId,
    long OrderCode,
    string Description,
    long AmountDue,
    long AmountPaid,
    string Currency,
    string Status,
    Uri? CheckoutUrl,
    string? QrCode,
    DateTime CreatedAt,
    DateTime? ExpiresAt,
    DateTime? PaidAt,
    DateTime? CancelledAt,
    string? CancellationReason);
