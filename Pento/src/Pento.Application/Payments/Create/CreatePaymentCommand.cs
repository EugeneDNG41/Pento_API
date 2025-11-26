
using FluentValidation;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Clock;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.PayOS;
using Pento.Domain.Abstractions;
using Pento.Domain.Payments;
using Pento.Domain.Shared;
using Pento.Domain.Subscriptions;

namespace Pento.Application.Payments.Create;

public sealed record CreatePaymentCommand(Guid SubscriptionPlanId) : ICommand<PaymentLinkResponse>;

internal sealed class CreatePaymentCommandValidator : AbstractValidator<CreatePaymentCommand> 
{
    public CreatePaymentCommandValidator()
    {
        RuleFor(c => c.SubscriptionPlanId)
            .NotEmpty().WithMessage("Subscription Plan Id is required.");
    }
}

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
        Subscription? subscription = await subscriptionRepository.GetByIdAsync(plan.SubscriptionId, cancellationToken);
        if (subscription == null)
        {
            return Result.Failure<PaymentLinkResponse>(SubscriptionErrors.SubscriptionNotFound);
        }
        string duration = plan.Duration == null ? "Lifetime" : $"{plan.Duration.Value}-{plan.Duration.Unit}";
        string description = $"Pento {duration} {subscription.Name}";
        var payment = Payment.Create(
            userId: userContext.UserId,
            subscriptionPlanId: plan.Id,
            paymentLinkId: null,
            description: description,
            amountDue: plan.Price.Amount,
            amountPaid: 0,
            currency: plan.Price.Currency,
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
