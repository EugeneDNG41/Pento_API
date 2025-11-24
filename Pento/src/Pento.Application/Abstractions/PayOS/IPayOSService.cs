using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PayOS.Models.V2.PaymentRequests;
using PayOS.Models.Webhooks;
using Pento.Domain.Abstractions;
using Pento.Domain.Payments;

namespace Pento.Application.Abstractions.PayOS;
#pragma warning disable CA1054 // URI-like parameters should not be strings
public interface IPayOSService
{
    Task<Result> CancelPaymentAsync(Guid paymentId, string? reason, CancellationToken cancellationToken);
    Task<Result> ConfirmWebhookAsync();
    Task<Result<PaymentLinkResponse>> CreatePaymentAsync(Payment payment, string returnUrl, string cancelUrl, CancellationToken cancellationToken);
    Task<Result> HandleWebhookAsync(Webhook webhook, CancellationToken cancellationToken);
}
