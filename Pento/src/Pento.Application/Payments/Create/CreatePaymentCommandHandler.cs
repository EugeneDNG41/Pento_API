using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.ThirdPartyServices.PayOS;
using Pento.Application.Abstractions.UtilityServices.Clock;
using Pento.Domain.Abstractions;
using Pento.Domain.Payments;
using Pento.Domain.Subscriptions;

namespace Pento.Application.Payments.Create;

internal sealed class CreatePaymentCommandHandler(
    IDateTimeProvider dateTimeProvider,
    IUserContext userContext,
    IPayOSService payOSService,
    IGenericRepository<Payment> paymentRepository,
    IGenericRepository<Subscription> subscriptionRepository,
    IGenericRepository<SubscriptionPlan> subscriptionPlanRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CreatePaymentCommand, PaymentLinkResponse>
{
    public async Task<Result<PaymentLinkResponse>> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
    {
        SubscriptionPlan? plan = await subscriptionPlanRepository.GetByIdAsync(request.SubscriptionPlanId, cancellationToken);
        if (plan == null)
        {
            return Result.Failure<PaymentLinkResponse>(SubscriptionErrors.SubscriptionPlanNotFound);
        }
        bool pendingOrProcessingPaymentExists = await paymentRepository.AnyAsync(
            p => p.UserId == userContext.UserId &&
                 p.SubscriptionPlanId == plan.Id &&
                 (p.Status == PaymentStatus.Pending || p.Status == PaymentStatus.Processing), cancellationToken);
        if (pendingOrProcessingPaymentExists)
        {
            return Result.Failure<PaymentLinkResponse>(PaymentErrors.PendingOrProcessingPayment);
        }
        Subscription? subscription = await subscriptionRepository.GetByIdAsync(plan.SubscriptionId, cancellationToken);
        if (subscription == null)
        {
            return Result.Failure<PaymentLinkResponse>(SubscriptionErrors.SubscriptionNotFound);
        }
        if (!subscription.IsActive)
        {
            return Result.Failure<PaymentLinkResponse>(SubscriptionErrors.InactiveSubscription);
        }
        string duration = plan.DurationInDays == null ? "Lifetime" : $"{plan.DurationInDays.Value}-day";
        string description = $"Pento {duration} {subscription.Name}";
        var payment = Payment.Create(
            userId: userContext.UserId,
            subscriptionPlanId: plan.Id,
            paymentLinkId: null,
            description: description,
            amountDue: plan.Amount,
            amountPaid: 0,
            currency: plan.Currency,
            checkoutUrl: null,
            qrCode: null,
            createdAt: dateTimeProvider.UtcNow);
        paymentRepository.Add(payment);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        Result<PaymentLinkResponse> result = await payOSService.CreatePaymentAsync(payment, cancellationToken);
        if (result.IsFailure)
        {
            return Result.Failure<PaymentLinkResponse>(result.Error);
        }
        return result.Value;
    }
}
