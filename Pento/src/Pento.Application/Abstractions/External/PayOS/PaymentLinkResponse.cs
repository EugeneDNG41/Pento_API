namespace Pento.Application.Abstractions.External.PayOS;

public sealed record PaymentLinkResponse(Guid PaymentId, Uri CheckoutUrl, string QrCode);

