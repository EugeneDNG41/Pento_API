using PayOS.Models.Webhooks;
using Pento.Domain.Abstractions;
using Pento.Domain.Payments;

namespace Pento.Application.Abstractions.External.PayOS;

public interface IPayOSService
{
    Task<Result> CancelPaymentAsync(Payment payment, string? reason, CancellationToken cancellationToken);
    Task<Result> ConfirmWebhookAsync();
    Task<Result<PaymentLinkResponse>> CreatePaymentAsync(Payment payment, CancellationToken cancellationToken);
    Task<Result<PaymentStatus>> GetPaymentLinkStatus(string PaymentLinkId);
    Task<Result> HandleWebhookAsync(Webhook webhook, CancellationToken cancellationToken);
}
