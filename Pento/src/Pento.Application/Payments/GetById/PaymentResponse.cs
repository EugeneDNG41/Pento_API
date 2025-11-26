namespace Pento.Application.Payments.GetById;

public sealed record PaymentResponse
{
    public Guid PaymentId { get; init; }
    public long OrderCode { get; init; }
    public string Description { get; init; }
    public string ProviderDescription { get; init; }
    public string AmountDue { get; init; }
    public string AmountPaid { get; init; }
    public string Status { get; init; }
    public Uri? CheckoutUrl { get; init; }
    public string? QrCode { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ExpiresAt { get; init; }
    public DateTime? PaidAt { get; init; }
    public DateTime? CancelledAt { get; init; }
    public string? CancellationReason { get; init; }
}
