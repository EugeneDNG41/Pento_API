namespace Pento.Application.Payments.GetAll;

public sealed record PaymentPreview
{
    public Guid PaymentId { get; init; }
    public long OrderCode { get; init; }
    public string Description { get; init; }
    public string AmountDue { get; init; }
    public string AmountPaid { get; init; }
    public string Status { get; init; }
    public DateTime CreatedAt { get; init; }
}
