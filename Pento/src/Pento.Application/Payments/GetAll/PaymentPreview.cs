namespace Pento.Application.Payments.GetAll;

public sealed record PaymentPreview(
    Guid PaymentId,
    long OrderCode,
    string Description,
    long Amount,
    string Currency,
    string Status,
    DateTime CreatedAt);
