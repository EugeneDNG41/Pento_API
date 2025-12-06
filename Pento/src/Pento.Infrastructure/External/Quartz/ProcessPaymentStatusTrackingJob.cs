using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.External.PayOS;
using Pento.Application.Abstractions.Utility.Clock;
using Pento.Domain.Abstractions;
using Pento.Domain.Payments;
using Quartz;

namespace Pento.Infrastructure.External.Quartz;

[DisallowConcurrentExecution]
internal sealed class ProcessPaymentStatusTrackingJob(
    IDateTimeProvider dateTimeProvider,
    IPayOSService payOSService,
    IGenericRepository<Payment> paymentRepository,
    IUnitOfWork unitOfWork) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var pendingPayments = (await paymentRepository.FindAsync(p => p.Status == PaymentStatus.Pending || p.Status == PaymentStatus.Processing, context.CancellationToken)).ToList();
        foreach (Payment payment in pendingPayments)
        {
            Result<PaymentStatus> statusResult = await payOSService.GetPaymentLinkStatus(payment.PaymentLinkId!);
            if (statusResult.IsSuccess && statusResult.Value != payment.Status)
            {
                switch (statusResult.Value)
                {
                    case PaymentStatus.Paid:
                        payment.MarkAsPaid(payment.AmountDue, dateTimeProvider.UtcNow);
                        break;
                    case PaymentStatus.Expired:
                        payment.MarkAsExpired();
                        break;
                    case PaymentStatus.Cancelled:
                        payment.MarkAsCancelled("Cancelled by user", dateTimeProvider.UtcNow);
                        break;
                    case PaymentStatus.Failed:
                        payment.MarkAsFailed();
                        break;
                    case PaymentStatus.Processing:
                        payment.MarkAsProcessing();
                        break;
                }
                paymentRepository.Update(payment);
                await unitOfWork.SaveChangesAsync(context.CancellationToken);
            }
        }       
    }
}
