
using FluentValidation;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.PayOS;
using Pento.Domain.Abstractions;
using Pento.Domain.Payments;
using Pento.Domain.Shared;

namespace Pento.Application.Payments.Create;

#pragma warning disable CA1054 // URI-like parameters should not be strings
public sealed record CreatePaymentCommand(long Amount, string Description, string ReturnUrl, string CancelUrl) : ICommand<PaymentLinkResponse>;

internal sealed class CreatePaymentCommandValidator : AbstractValidator<CreatePaymentCommand> 
{
    public CreatePaymentCommandValidator()
    {
        RuleFor(c => c.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than zero.");
        RuleFor(c => c.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");
        RuleFor(c => c.ReturnUrl)
            .NotEmpty().WithMessage("Return Url is required.")
            .Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute)).WithMessage("ReturnUrl must be a valid absolute URI.");
        RuleFor(c => c.CancelUrl)
            .NotEmpty().WithMessage("Cancel Url is required.")
            .Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute)).WithMessage("CancelUrl must be a valid absolute URI.");
    }
}

internal sealed class CreatePaymentCommandHandler(
    IUserContext userContext,
    IPayOSService payOSService,
    IGenericRepository<Payment> paymentRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CreatePaymentCommand, PaymentLinkResponse>
{
    public async Task<Result<PaymentLinkResponse>> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = Payment.Create(
            userId: userContext.UserId,
            paymentLinkId: null,
            description: request.Description,
            amountDue: request.Amount,
            amountPaid: 0,
            currency: Currency.Vnd,
            checkoutUrl: null,
            qrCode: null,
            createdAt: DateTime.UtcNow);
        paymentRepository.Add(payment);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        Result<PaymentLinkResponse> result = await payOSService.CreatePaymentAsync(payment, request.ReturnUrl, request.CancelUrl, cancellationToken);
        if (result.IsFailure)
        {
            return Result.Failure<PaymentLinkResponse>(result.Error);
        }
        return result.Value;
    }
}
