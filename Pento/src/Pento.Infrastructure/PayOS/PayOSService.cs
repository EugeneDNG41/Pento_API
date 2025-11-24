using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Microsoft.Extensions.Options;
using PayOS;
using PayOS.Exceptions;
using PayOS.Models.V2.PaymentRequests;
using PayOS.Models.Webhooks;
using Pento.Application.Abstractions.PayOS;
using Pento.Domain.Abstractions;

namespace Pento.Infrastructure.PayOS;

internal sealed class PayOSService(IOptions<PayOSCustomOptions> options) : IPayOSService
{
    public async Task<Result<CreatePaymentLinkResponse>> CreatePaymentAsync(long amount, string description)
    {
        try
        {


            using var client = new PayOSClient(new PayOSOptions
            {
                ClientId = options.Value.ClientId,
                ApiKey = options.Value.ApiKey,
                ChecksumKey = options.Value.ChecksumKey
            });
            var paymentRequest = new CreatePaymentLinkRequest
            {
                OrderCode = 123,
                Amount = amount,
                Description = description,
                ReturnUrl = options.Value.ReturnUrl,
                CancelUrl = options.Value.CancelUrl
            };
            CreatePaymentLinkResponse response = await client.PaymentRequests.CreateAsync(paymentRequest);

            return response;
        }
        catch (ApiException ex)
        {
            return Result.Failure<CreatePaymentLinkResponse>(PayOSErrors.PaymentCreationFailed(ex.Message));
        }
        catch (PayOSException ex)
        {
            return Result.Failure<CreatePaymentLinkResponse>(PayOSErrors.PaymentCreationFailed(ex.Message));
        }
    }
    public async Task<Result> HandleWebhookAsync(Webhook webhook)
    {
        try
        {
            using var client = new PayOSClient(new PayOSOptions
            {
                ClientId = options.Value.ClientId,
                ApiKey = options.Value.ApiKey,
                ChecksumKey = options.Value.ChecksumKey
            });
#pragma warning disable S1481 // Unused local variables should be removed
            WebhookData verifiedData = await client.Webhooks.VerifyAsync(webhook);

            return Result.Success();
        }
        catch
        {
            return Result.Failure(PayOSErrors.InvalidWebhook);
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
            ConfirmWebhookResponse response = await client.Webhooks.ConfirmAsync(options.Value.WebhookUrl);
            return Result.Success();
        }
        catch (WebhookException)
        {
            return Result.Failure(PayOSErrors.WebhookConfirmationFailed);
        }
    }
}
public sealed class PayOSCustomOptions
{
    [Required]
    public string ClientId { get; set; }
    [Required]
    public string ApiKey { get; set; }
    [Required]
    public string ChecksumKey { get; set; }
    [Required]
    public string WebhookUrl { get; set; }
    [Required]
    public string ReturnUrl { get; set; }
    [Required]
    public string CancelUrl { get; set; }

}
