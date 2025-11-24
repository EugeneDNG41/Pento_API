using System.Reflection.Metadata.Ecma335;
using Microsoft.Extensions.Options;
using PayOS;
using PayOS.Exceptions;
using PayOS.Models.V2.PaymentRequests;
using PayOS.Models.Webhooks;
using Pento.Application.Abstractions.Clock;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.PayOS;
using Pento.Domain.Abstractions;
using Pento.Domain.Payments;

namespace Pento.Infrastructure.PayOS;

internal sealed class PayOSService(
    IOptions<PayOSCustomOptions> options, 
    IDateTimeProvider dateTimeProvider,
    IGenericRepository<Payment> paymentRepo,
    IUnitOfWork unitOfWork) : IPayOSService
{
    public async Task<Result> CancelPaymentAsync(Guid paymentId, string? reason, CancellationToken cancellationToken)
    {
        try
        {
            Payment? payment = await paymentRepo.GetByIdAsync(paymentId, cancellationToken);
            if (payment is null)
            {
                return Result.Failure(PaymentErrors.PaymentNotFound);
            }
            if (!string.IsNullOrWhiteSpace(payment.PaymentLinkId))
            {
                using var client = new PayOSClient(new PayOSOptions
                {
                    ClientId = options.Value.ClientId,
                    ApiKey = options.Value.ApiKey,
                    ChecksumKey = options.Value.ChecksumKey
                });
                await client.PaymentRequests.CancelAsync(payment.PaymentLinkId, reason);
            }
            payment.MarkAsCancelled(reason, dateTimeProvider.UtcNow);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch
        {
            return Result.Failure(PaymentErrors.PaymentCancellationFailed);
        }
    }
    public async Task<Result<PaymentLinkResponse>> CreatePaymentAsync(Payment payment, string returnUrl, string cancelUrl, CancellationToken cancellationToken)
    {
        try
        {
            using var client = new PayOSClient(new PayOSOptions
            {
                ClientId = options.Value.ClientId,
                ApiKey = options.Value.ApiKey,
                ChecksumKey = options.Value.ChecksumKey
            });
            DateTime expiresAt = dateTimeProvider.UtcNow.AddMinutes(5);
            var paymentRequest = new CreatePaymentLinkRequest
            {
                OrderCode = payment.OrderCode,
                Amount = payment.Amount,
                Description = payment.Description,
                ReturnUrl = returnUrl,
                CancelUrl = cancelUrl,
                ExpiredAt = (long)expiresAt.Subtract(DateTime.UnixEpoch).TotalSeconds
            };
            CreatePaymentLinkResponse response = await client.PaymentRequests.CreateAsync(paymentRequest);
            payment.UpdatePaymentLink(response.PaymentLinkId, new Uri(response.CheckoutUrl), response.QrCode, expiresAt);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return new PaymentLinkResponse(new Uri(response.CheckoutUrl), response.QrCode);
        }
        catch
        {
            payment.MarkAsFailed();
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Failure<PaymentLinkResponse>(PaymentErrors.PaymentCreationFailed);
        }

    }
    public async Task<Result> HandleWebhookAsync(Webhook webhook, CancellationToken cancellationToken)
    {
        try
        {
            using var client = new PayOSClient(new PayOSOptions
            {
                ClientId = options.Value.ClientId,
                ApiKey = options.Value.ApiKey,
                ChecksumKey = options.Value.ChecksumKey
            });

            WebhookData data = await client.Webhooks.VerifyAsync(webhook);
            if (data.Code.Equals("00", StringComparison.Ordinal))
            {
                PaymentLink paymentLink = await client.PaymentRequests.GetAsync(data.PaymentLinkId);
                Payment? payment = (await paymentRepo.FindAsync(p => p.PaymentLinkId == data.PaymentLinkId, cancellationToken)).SingleOrDefault();
                if (payment is null)
                {
                    return Result.Failure(PaymentErrors.PaymentNotFound);
                }
                switch (paymentLink.Status)
                {
                    case PaymentLinkStatus.Paid:
                    case PaymentLinkStatus.Underpaid:
                        payment.MarkAsPaid(paymentLink.AmountPaid, dateTimeProvider.UtcNow);
                        break;
                    case PaymentLinkStatus.Cancelled:
                        payment.MarkAsCancelled(paymentLink.CancellationReason, dateTimeProvider.UtcNow);
                        break;
                    case PaymentLinkStatus.Expired:
                        payment.MarkAsExpired();
                        break;
                    case PaymentLinkStatus.Processing:
                        payment.MarkAsProcessing();
                        break;
                    case PaymentLinkStatus.Failed:
                        payment.MarkAsFailed();
                        break;
                }
                await unitOfWork.SaveChangesAsync(cancellationToken);
            }           
            return Result.Success();
        }
        catch (WebhookException)
        {
            return Result.Failure(PaymentErrors.InvalidWebhook);
        }
    }
    public async Task<Result> ConfirmWebhookAsync()
    {
        try
        {
            using var client = new PayOSClient(new PayOSOptions
            {
                ClientId = options.Value.ClientId,
                ApiKey = options.Value.ApiKey,
                ChecksumKey = options.Value.ChecksumKey
            });
            await client.Webhooks.ConfirmAsync(options.Value.WebhookUrl);
            return Result.Success();
        }
        catch (WebhookException)
        {
            return Result.Failure(PaymentErrors.WebhookConfirmationFailed);
        }
    }
}
