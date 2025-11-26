namespace Pento.Application.Payments.GetAll;

public sealed record PaymentPreview
{
    public Guid PaymentId { get; init; }
    public long OrderCode { get; init; }
    public string Description { get; init; }
    public string Amount { get; init; }
    public string Status { get; init; }
    public DateTime CreatedAt { get; init; }
}
