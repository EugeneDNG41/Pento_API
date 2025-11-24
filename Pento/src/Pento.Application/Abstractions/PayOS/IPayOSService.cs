using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PayOS.Models.V2.PaymentRequests;
using PayOS.Models.Webhooks;
using Pento.Domain.Abstractions;

namespace Pento.Application.Abstractions.PayOS;

public interface IPayOSService
{
    Task<Result> ConfirmWebhookAsync();
    Task<Result<CreatePaymentLinkResponse>> CreatePaymentAsync(long amount, string description);
    Task<Result> HandleWebhookAsync(Webhook webhook);
}
